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

        Vector3 moveToPosition = new Vector3(hit.point.x, 0, hit.point.z);
        List<Vector3> targetPositionList = MathUtility.GetPositionsAround(moveToPosition, 0.2f, 0.7f, selectedUnits.Count);

        int targetPositionListIndex = 0;
        foreach (UnitGeneral unit in selectedUnits)
        {
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
            Vector3 target = new Vector3(targetPositionList[targetPositionListIndex].x, unit.transform.position.y
                , targetPositionList[targetPositionListIndex].z);
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
