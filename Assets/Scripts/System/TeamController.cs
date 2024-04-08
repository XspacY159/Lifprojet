using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] protected TeamName team;
    [SerializeField] protected List<UnitGeneral> units = new List<UnitGeneral>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UnitGeneral unit = transform.GetChild(0).GetComponent<UnitGeneral>();
            if (!units.Contains(unit))
                units.Add(unit);
        }
    }

    public void AddUnit(UnitGeneral unit)
    {
        if(!units.Contains(unit))
            units.Add(unit);
    }

    virtual public void RemoveUnit(UnitGeneral unit)
    {
        if (units.Contains(unit))
            units.Remove(unit);
    }

    public List<UnitGeneral> GetUnits ()
    {
        return units;
    }

    public TeamName GetTeamName()
    {
        return team; 
    }
}
