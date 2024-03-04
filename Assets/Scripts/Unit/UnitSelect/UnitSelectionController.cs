using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    public Dictionary<GameObject, UnitGeneral> unitsList { get; private set; } 
        = new Dictionary<GameObject, UnitGeneral>(); //Dictionnary of player's units
    private List<UnitGeneral> unitsSelected = new List<UnitGeneral>();

    // singleton of the selection
    private static UnitSelectionController _instance;
    public static UnitSelectionController Instance { get { return _instance; } }

    public event Action changeUnitSelection;

    private void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public List<UnitGeneral> GetUnitsSelected()
    {
        return unitsSelected; 
    }

    public void AddUnit(UnitGeneral unit)
    {
        if(!unitsList.ContainsKey(unit.gameObject))
            unitsList.Add(unit.gameObject, unit);
    }

    public void RemoveUnit(UnitGeneral unit)
    {
        if (unitsList.ContainsKey(unit.gameObject))
            unitsList.Remove(unit.gameObject);
    }

    public List<UnitGeneral> GetSelectedUnits()
    {
        return unitsSelected;
    }

    public void ClickSelect(GameObject unitToAdd)   //player selects only one unit
    {
        DeselectAll();
        if (!unitsList.ContainsKey(unitToAdd)) return;

        if (unitsList[unitToAdd].GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) return;

        unitsSelected.Add(unitsList[unitToAdd]);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true); //activates the highlight object
        changeUnitSelection?.Invoke();
    }

    public void ShiftSelect(GameObject unitToAdd)   //player selects many units with shift pressed
    {
        if (!unitsList.ContainsKey(unitToAdd)) return;

        if (unitsList[unitToAdd].GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) return;

        if (!unitsSelected.Contains(unitsList[unitToAdd]))
        {
            unitsSelected.Add(unitsList[unitToAdd]);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true); //activates the highlight object
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false); //deactivates the highlight object
            if (!unitsList.ContainsKey(unitToAdd)) return;
            unitsSelected.Remove(unitsList[unitToAdd]);
        }

        changeUnitSelection?.Invoke();
    }

    public void DragSelect(List<GameObject> unitsToAdd)    //player selects many units with dragged box
    {
        int addedUnitsCount = 0;
        foreach (GameObject unitToAdd in unitsToAdd) 
        {
            if (!unitsList.ContainsKey(unitToAdd)) continue;

            if (unitsList[unitToAdd].GetTeam() != GameInfoManager.Instance.GetPlayerTeam()) continue;

            if (!unitsSelected.Contains(unitsList[unitToAdd]))  //the unit is not yet in the selected list
            {
                unitsSelected.Add(unitsList[unitToAdd]);
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
