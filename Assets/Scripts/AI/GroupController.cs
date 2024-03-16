using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour
{
    private List<UnitGeneral> group = new List<UnitGeneral> ();
    private Guid groupID = System.Guid.NewGuid();

    // Update is called once per frame
    void Update()
    {
        
    }

    public Guid GetID()
    {
        return groupID;
    }

    public Guid GetMessageAddress()
    {
        return group[0].GetUnitID();
    }

    public void AddUnit(UnitGeneral unit)
    {
        if(!group.Contains(unit))
            group.Add(unit);
    }

    public void RemoveUnit(UnitGeneral unit)
    {
        if(group.Contains(unit))
            group.Remove(unit);
    }

    public bool UnitInGroup(UnitGeneral unit)
    { 
        return group.Contains(unit); 
    }

    public void AttackUnit(UnitGeneral targetUnit)
    {
        foreach (UnitGeneral unit in group)
        {
            unit.AttackUnit(targetUnit);
        }
    }

    public void GoTo(Vector3 pos)
    {
        foreach (UnitGeneral unit in group)
        {
            unit.GoTo(pos);
        }
    }
}
