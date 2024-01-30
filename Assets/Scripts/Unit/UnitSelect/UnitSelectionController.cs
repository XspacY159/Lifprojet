using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    public List<GameObject> unitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    // singleton of the selection
    private static UnitSelectionController _instance;
    public static UnitSelectionController Instance {  get { return _instance; } }

    private void Awake()
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

    public void ClickSelect(GameObject unitToAdd)   //player selects only one unit
    {
        DeselectAll();
        unitsSelected.Add(unitToAdd);
    }

    public void ShiftSelect(GameObject unitToAdd)   //player selects many units with shift pressed
    {
        if (!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
        }
        else
        {
            unitsSelected.Remove(unitToAdd);
        }
    }

    public void DragSelect(GameObject unitToAdd)    //player selects many units with dragged box
    {

    }

    public void DeselectAll()
    {
        unitsSelected.Clear();
    }

    public void Deselect(GameObject unitToDeselect)
    {

    }
}
