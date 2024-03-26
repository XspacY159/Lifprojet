using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController
{
    private List<UnitGeneral> group = new List<UnitGeneral> ();
    private Guid groupID = System.Guid.NewGuid();
    public int currentActionPriority;
    private GameObject groupTree;

    public GroupController(GameObject _groupTree, AITeamController _team)
    {
        Debug.Log("group created");
        groupTree = _groupTree;

        GroupBehaviourTreeSolver tree = groupTree.GetComponent<GroupBehaviourTreeSolver>();
        tree.Setup(_team, this);
    }

    public Guid GetID()
    {
        return groupID;
    }

    public Guid GetMessageAddress()
    {
        return group[0].GetUnitID();
    }

    public GameObject GetGroupTree()
    { 
        return groupTree;
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
