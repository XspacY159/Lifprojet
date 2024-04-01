using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputProvider.Graph;

public class GroupController
{
    private List<UnitGeneral> group = new List<UnitGeneral> ();
    private Guid groupID = System.Guid.NewGuid();
    private AITeamController team;
    public int currentActionPriority;
    private GameObject groupTree;

    private int currentUnitIndex;

    public GroupController(GameObject _groupTree, AITeamController _team)
    {
        Debug.Log("group created");
        groupTree = _groupTree;
        team = _team;

        GroupBehaviourTreeSolver tree = groupTree.GetComponent<GroupBehaviourTreeSolver>();
        tree.Setup(_team, this);

        currentUnitIndex = 0;
    }

    public Guid GetID()
    {
        return groupID;
    }

    public Guid GetMessageAddress()
    {
        return GetOneUnit().GetUnitID();
    }

    public Vector3 GetGroupPosition()
    {
        UnitGeneral unit = GetOneUnit();
        if (unit != null)
        {
            return unit.transform.position; //might need a better choice
        }
        else
            return Vector3.zero;
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
            if (targetUnit != null)
                unit.AttackUnit(targetUnit);
            else
                break;
        }
    }

    public void GoTo(Vector3 pos)
    {
        foreach (UnitGeneral unit in group)
        {
            unit.GoTo(pos);
        }
    }

    public UnitGeneral FindAdversary()
    {
        foreach(UnitGeneral unit in group)
        {
            if (unit == null) continue;
            foreach (Collider unitInRange in unit.UnitsInRange())
            {
                if (unitInRange.gameObject == unit.gameObject) continue;
                UnitGeneral tmp = UnitManager.Instance.GetUnit(unitInRange.gameObject);
                if (tmp.GetTeam() != unit.GetTeam())
                {
                    return tmp;
                }
            }
        }
        return null;
    }

    public UnitMessages ReadMessages()
    {
        foreach (UnitGeneral unit in group)
        {
            UnitMessages tmp = team.ReadMessage(unit.GetUnitID());
            if (tmp != null) return tmp;
        }
        return null;
    }

    //this function is used to get one unit in the group for purposes like position
    private UnitGeneral GetOneUnit()
    {
        while (currentUnitIndex < group.Count)
        {
            if (group[currentUnitIndex] != null)
            {
                return group[currentUnitIndex];
            }
            currentUnitIndex++;
        }
        return null;
    }
}
