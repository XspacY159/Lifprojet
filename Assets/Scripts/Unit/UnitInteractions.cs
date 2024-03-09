using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask poiLayer;
    [SerializeField] private LayerMask unitLayer;
    private List<UnitGeneral> selectedUnits = new List<UnitGeneral>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //traces a ray from the camera to the scene, according to the position of the mouse cursor

            //Ordre des if/elseif important
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) // Click on Unit
            {
                MakeUnitsAttack(UnitManager.Instance.GetUnit(hit.collider.gameObject));
                MakeUnitsStopTryToInteract();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, poiLayer)) // Click on POI
            {
                MoveUnits(hit);
                MakeUnitsTryToInteract(hit.collider.GetComponent<POI>());
                MakeUnitsStopAttack();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer)) //Click on tile
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile == null || !tile.GetTileType().walkable)
                    return;

                MoveUnits(hit);
                MakeUnitsStopTryToInteract();
                MakeUnitsStopAttack();
            }
        }
    }

    private void MoveUnits(RaycastHit hit)
    {
        selectedUnits = UnitSelectionController.Instance.GetSelectedUnits();

        Vector3 moveToPosition = new Vector3(hit.point.x, 0, hit.point.z);
        List<Vector3> targetPositionList;

        int targetPositionListIndex = 0;

        if(selectedUnits.Count == 1)
        {
            targetPositionList = MathUtility.GetPositionsAround(moveToPosition, 0.2f, 0.4f, 10);
            targetPositionListIndex = UnityEngine.Random.Range(0, targetPositionList.Count);
            Vector3 target = new Vector3(targetPositionList[targetPositionListIndex].x, selectedUnits[0].transform.position.y,
                targetPositionList[targetPositionListIndex].z);
            selectedUnits[0].GoTo(target);

            return;
        }

        targetPositionList = MathUtility.GetPositionsAround(moveToPosition, 0.2f, 0.7f, selectedUnits.Count);
        foreach (UnitGeneral unit in selectedUnits)
        {
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
            Vector3 target = new Vector3(targetPositionList[targetPositionListIndex].x, unit.transform.position.y,
                targetPositionList[targetPositionListIndex].z);
            unit.GoTo(target);
        }
    }

    private void MakeUnitsTryToInteract(POI poi)
    {
        if (poi == null) return;
        poi.transform.GetChild(0).gameObject.SetActive(true);

        foreach (UnitGeneral unit in selectedUnits)
        {
            unit.TryInteract(poi);
        }
    }

    private void MakeUnitsStopTryToInteract()
    {
        foreach (UnitGeneral unit in selectedUnits)
        {
            unit.StopTryInteract();
        }
    }

    private void MakeUnitsAttack(UnitGeneral unitToAttack)
    {
        //TODO : Highlight the targeted enemy's unit
        selectedUnits = UnitSelectionController.Instance.GetSelectedUnits();

        foreach (UnitGeneral unit in selectedUnits)
        {
            if (unit.GetTeam() == unitToAttack.GetTeam()) continue;
            unit.AttackUnit(unitToAttack);
        }
    }

    private void MakeUnitsStopAttack()
    {
        //TODO : Remove the highlight of the targeted enemy's unit when no player's unit are targeting it
        foreach (UnitGeneral unit in selectedUnits)
        {
            unit.StopAttackUnit();
        }
    }
}
