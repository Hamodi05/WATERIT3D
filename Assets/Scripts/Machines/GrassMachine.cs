using UnityEngine;

public class GrassMachine : MonoBehaviour
{

    [SerializeField]
    private float mapSizeX = 15f; // Size of the map along the X-axis

    [SerializeField]
    private float mapSizeZ = 15f; // Size of the map along the Z-axis

    [SerializeField]
    private Vector3 MapCenter = new Vector3(-1.5f, 0.015f, -1.5f);


    public LayerMask terrainLayer; // Layer mask for terrain objects
    public GameObject grassPrefab; // The grass prefab to place
    private Grid grid; // Reference to the grid component
    private PlacementSystem placementSystem; // Reference to the placement system
    private int currentMode; // Current mode for grass placement (1, 2, or 3)

    private void Start()
    {
        grid = FindObjectOfType<Grid>(); // Find the grid component in the scene
        placementSystem = FindObjectOfType<PlacementSystem>(); // Find the placement system component in the scene
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Change mode to 1
        {
            currentMode = 1;
            Debug.Log("Mode changed to 1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Change mode to 2
        {
            currentMode = 2;
            Debug.Log("Mode changed to 2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Change mode to 3
        {
            currentMode = 3;
            Debug.Log("Mode changed to 3");
        }

        if (!placementSystem.IsPlacementActive()) // Check if placement system is active
            return;

        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) // Check if ray hits terrain
            {
                if (IsWithinMap(hit.point))
                {
                    GrowGrass(hit.point);
                }
                else
                {
                    Debug.LogWarning("Grass cannot be placed outside the map!");
                }
            }
        }
    }

    private void GrowGrass(Vector3 position)
    {
        if (!placementSystem.IsPlacementActive() || !placementSystem.IsPlacingGrassMachine()) // Check if placement system is active and placing grass machine
            return;

        // Snap the position to the grid
        Vector3Int snappedPosition = grid.WorldToCell(position);

        // Get the center position of the snapped cell
        Vector3 centerPosition = grid.GetCellCenterWorld(snappedPosition);

        // Spacing between grass instances
        float spacing = 1.0f; // Adjust as needed

        // Remove existing grass prefabs that overlap with the new grass placement
        RemoveExistingGrass(centerPosition, spacing);

        // Determine the mode-specific shape of grass placement
        switch (currentMode)
        {
            case 1:
                PlaceGrassMode1(centerPosition, spacing);
                break;
            case 2:
                PlaceGrassMode2(centerPosition, spacing);
                break;
            case 3:
                PlaceGrassMode3(centerPosition, spacing);
                break;
            default:
                Debug.Log("Invalid mode selected!");
                break;
        }
    }

    private void PlaceGrassMode1(Vector3 centerPosition, float spacing)
    {
        // Iterate over a 3x3 grid in x and z dimensions (ignoring y-axis)
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 grassPosition = centerPosition + new Vector3(x * spacing, 0, z * spacing);
                InstantiateGrass(grassPosition);
            }
        }
    }

    private void PlaceGrassMode2(Vector3 centerPosition, float spacing)
    {
        // Iterate over a 3x3 grid in x and z dimensions (ignoring y-axis)
        for (int x = -5; x <= 0; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Place grass only on alternate rows
                Vector3 grassPosition = centerPosition + new Vector3(x * spacing, 0, z * spacing);
                InstantiateGrass(grassPosition);
            }
        }
    }

    private void PlaceGrassMode3(Vector3 centerPosition, float spacing)
    {
        // Iterate over a 3x3 grid in x and z dimensions (ignoring y-axis)
        for (int x = -5; x <= 0; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Place grass in a diagonal pattern
                Vector3 grassPosition = centerPosition + new Vector3(x * spacing, 0, z * spacing);
                InstantiateGrass(grassPosition);
            }
        }
    }

    private void InstantiateGrass(Vector3 position)
    {
        // Snap the position to the nearest grid cell
        Vector3Int grassCellPosition = grid.WorldToCell(position);

        // Check if the grass is within the map boundaries
        if (!IsWithinMap(position))
            return;

        // Check if there's already grass at this position
        if (!IsGrassAtPosition(grassCellPosition))
        {
            // Instantiate grass prefab at the center of the calculated grid cell with Y axis position set to 0
            position = grid.GetCellCenterWorld(grassCellPosition);
            position.y = 0.01f; // Set Y component to 0
            GameObject grassObject = Instantiate(grassPrefab, position, Quaternion.identity);
            grassObject.tag = "Grass"; // Ensure that the newly instantiated grass is tagged appropriately
        }
    }

    private bool IsWithinMap(Vector3 position)
    {
        // Define the map size
        float MapSizeX = mapSizeX; // Size of the map along the X-axis
        float MapSizeZ = mapSizeZ; // Size of the map along the Z-axis

        // Get the center of the map
        Vector3 mapCenter = MapCenter;

        // Calculate the minimum and maximum boundaries of the map
        Vector3 minBound = mapCenter - new Vector3(MapSizeX / 2f, 0f, MapSizeZ / 2f);
        Vector3 maxBound = mapCenter + new Vector3(MapSizeX / 2f, 0f, MapSizeZ / 2f);

        // Check if the position is within the map boundaries
        return position.x >= minBound.x && position.x <= maxBound.x &&
               position.z >= minBound.z && position.z <= maxBound.z;
    }

    private void RemoveExistingGrass(Vector3 centerPosition, float spacing)
    {
        // Iterate over a 3x3 grid in x and z dimensions (ignoring y-axis)
        for (int x = -5; x <= 0; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 grassPosition = centerPosition + new Vector3(x * spacing, 0, z * spacing);

                // Check if the grass is within the map boundaries
                if (IsWithinMap(grassPosition))
                {
                    RemoveGrassAtPosition(grassPosition);
                }
            }
        }
    }

    private void RemoveGrassAtPosition(Vector3 position)
    {
        // Find all grass instances at the specified position
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f); // Using a small sphere as a trigger area

        // Iterate through all colliders found
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Grass"))
            {
                Destroy(collider.gameObject);
            }
        }
    }

    private bool IsGrassAtPosition(Vector3Int position)
    {
        // Find all grass instances in the scene
        GameObject[] existingGrass = GameObject.FindGameObjectsWithTag("Grass");

        // Iterate through each existing grass instance
        foreach (GameObject grass in existingGrass)
        {
            // Check if the grass is at the specified position
            if (grid.WorldToCell(grass.transform.position) == position)
            {
                return true;
            }
        }

        return false;
    }
}