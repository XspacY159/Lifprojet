using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    private List<UnitGeneral> unitsSelected = new List<UnitGeneral>();

    // singleton of the selection
    public static UnitSelectionController Instance;

    public event Action changeUnitSelection;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public List<UnitGeneral> GetUnitsSelected()
    {
        return unitsSelected; 
    }

    public List<UnitGeneral> GetSelectedUnits()
    {
        return unitsSelected;
    }

    public void ClickSelect(GameObject unitToAdd)   //player selects only one unit
    {
        DeselectAll();
        UnitGeneral unit = UnitManager.Instance.GetUnit(unitToAdd);
        if (unit == null) return;

        if (unit.GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) return;

        unitsSelected.Add(unit);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true); //activates the highlight object
        changeUnitSelection?.Invoke();
    }

    public void ShiftSelect(GameObject unitToAdd)   //player selects many units with shift pressed
    {
        UnitGeneral unit = UnitManager.Instance.GetUnit(unitToAdd);
        if (unit == null) return;

        if (unit.GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) return;

        if (!unitsSelected.Contains(unit))
        {
            unitsSelected.Add(unit);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true); //activates the highlight object
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false); //deactivates the highlight object
            if (unit == null) return;
            unitsSelected.Remove(unit);
        }

        changeUnitSelection?.Invoke();
    }

    public void DragSelect(List<GameObject> unitsToAdd)    //player selects many units with dragged box
    {
        int addedUnitsCount = 0;
        foreach (GameObject unitToAdd in unitsToAdd) 
        {
            UnitGeneral unit = UnitManager.Instance.GetUnit(unitToAdd);
            if (unit == null) continue;

            if (unit.GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) continue;

            if (!unitsSelected.Contains(unit))  //the unit is not yet in the selected list
            {
                unitsSelected.Add(unit);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true); //activates the highlight object
                addedUnitsCount++;
            }
        }
        if(addedUnitsCount > 0) changeUnitSelection?.Invoke();
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false); //deactivates the highlight object
        }
        unitsSelected.Clear();

        changeUnitSelection?.Invoke();
    }

    public void Deselect(UnitGeneral unitToDeselect)
    {
        if (!unitsSelected.Contains(unitToDeselect)) return;
        unitsSelected.Remove(unitToDeselect);
    }
}
