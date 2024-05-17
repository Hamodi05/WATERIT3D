using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private bool isPlacementActive = false;
    private bool isPlacingGrassMachine = false;
    private bool isPlacingTreemachine = false;




    private void Start()
    {
        StopPlacement();
    }
    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        if (ID == 2)
        {
            isPlacingGrassMachine = true;
        }
        else
        {
            isPlacingGrassMachine = false;
        }

        if (ID == 3)
        { isPlacingTreemachine = true; }
        else
        {
            isPlacingTreemachine = false;
        }

        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        isPlacementActive = true;

    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Offset the position by half of the cell size
        Vector3 offset = new Vector3(0.5f, 0f, 0.5f); // Assuming cell size is (1, 1, 1)
        Vector3 finalPosition = grid.CellToWorld(gridPosition) + offset;

        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = finalPosition;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        isPlacementActive = false;

    }


    private void Update()
    {
        if (selectedObjectIndex < 0)
            return; 
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
    public bool IsPlacementActive()
    {
        return isPlacementActive;
    }
    public bool IsPlacingGrassMachine()
    {
        return isPlacingGrassMachine;
    }
    public bool IsPlacingTreeMachine()
    {
        return isPlacingTreemachine;
    }

}
