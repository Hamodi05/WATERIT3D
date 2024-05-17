using UnityEngine;

public class TreeMachine : MonoBehaviour
{
    public LayerMask terrainLayer; // Layer mask for terrain objects
    public GameObject treePrefab; // The tree prefab to place
    private Grid grid; // Reference to the grid component
    private PlacementSystem placementSystem; // Reference to the placement system
    private PlacementSystem IsPlacingTreeMachine;
    private void Start()
    {
        grid = FindObjectOfType<Grid>(); // Find the grid component in the scene
        placementSystem = FindObjectOfType<PlacementSystem>(); // Find the placement system component in the scene
    }

    private void Update()
    {
        if (!placementSystem.IsPlacementActive()) // Check if placement system is active
            return;

        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) // Check if ray hits terrain
            {
                GrowTree(hit.point);
            }
        }
    }

    private void GrowTree(Vector3 position)
    {
        // Check if the object being placed is a tree machine
        if (!placementSystem.IsPlacementActive() || !placementSystem.IsPlacingTreeMachine())
            return;

        // Snap the position to the grid
        Vector3Int snappedPosition = grid.WorldToCell(position);

        // Offset the position by half of the cell size
        Vector3 offset = new Vector3(0.5f, 0f, 0.5f); // Assuming cell size is (1, 1, 1)
        Vector3 finalPosition = grid.CellToWorld(snappedPosition) + offset;

        // Instantiate tree prefab at the snapped position
        GameObject treeObject = Instantiate(treePrefab, finalPosition, Quaternion.identity);

        // You may want to add additional logic here to adjust tree properties or perform other actions
    }
}