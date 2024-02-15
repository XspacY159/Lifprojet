using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask poiLayer;
    private List<UnitGeneral> selectedUnits = new List<UnitGeneral>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //traces a ray from the camera to the scene, according to the position of the mouse cursor

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground)) //Click on tile
            {
                MoveUnits(hit);
                MakeUnitTopTryToInteract();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, poiLayer)) // Click on POI
            {
                MoveUnits(hit);
                MakeUnitsTryToInteract(hit.collider.GetComponent<POI>());
            }
        }
    }

    private void MoveUnits(RaycastHit hit)
    {
        selectedUnits = UnitSelectionController.Instance.GetSelectedUnits();

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null || !tile.GetTileType().walkable)
            return;
    
        foreach (UnitGeneral unit in selectedUnits)
        {
            Vector3 target = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
            unit.GoTo(target);
        }
    }

    private void MakeUnitsTryToInteract(POI poi)
    {
        foreach (UnitGeneral unit in selectedUnits)
        {
            unit.TryInteract(poi);
        }
    }

    private void MakeUnitTopTryToInteract()
    {
        foreach (UnitGeneral unit in selectedUnits)
        {
            unit.StopTryInteract();
        }
    }
}
