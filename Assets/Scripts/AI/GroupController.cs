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

    //gets the position of the first (non null) unit in the group
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
    //gets the position of the closest unit to a point
    public UnitGeneral GetClosestUnit(Vector3 point)
    {
        UnitGeneral closestUnit = null;
        float minDist = 1000;

        foreach (UnitGeneral unit in group)
        {
            if (unit != null)
            {
                float dist = Vector3.Distance(unit.transform.position, point);
                if (dist < minDist) 
                {
                    minDist = dist;
                    closestUnit = unit;
                }
            }
        }
        return closestUnit;
    }

    public void TryInteract(UnitGeneral unit, POI poi)
    {
        unit.TryInteract(poi);
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
        List<Vector3> targetPositionList;
        int targetPositionListIndex = 0;

        targetPositionList = MathUtility.GetPositionsAround(pos, 0.2f, 0.7f, group.Count);
        foreach (UnitGeneral unit in group)
        {
            targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
            Vector3 target = new Vector3(targetPositionList[targetPositionListIndex].x, unit.transform.position.y,
                targetPositionList[targetPositionListIndex].z);
            target.x = Mathf.Clamp(target.x, 0, TerrainManager.Instance.GetTerrainSize().x - 1);
            target.z = Mathf.Clamp(target.z, 0, TerrainManager.Instance.GetTerrainSize().y - 1);
            unit.GoTo(target);
        }
        /*
        foreach (UnitGeneral unit in group)
        {
            unit.GoTo(pos);
        }*/
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
    public UnitGeneral GetOneUnit()
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

    public void Roam(float maxDistance, Vector3 fixedPoint)    //move to unit controller
    {
        if (!TimerManager.StartTimer(3, "RandRoam" + GetID()))
        {
            float randX = Mathf.Clamp(fixedPoint.x + UnityEngine.Random.Range(-maxDistance, maxDistance),
                0, TerrainManager.Instance.GetTerrainSize().x - 1);
            float randZ = Mathf.Clamp(fixedPoint.z + UnityEngine.Random.Range(-maxDistance, maxDistance),
                0, TerrainManager.Instance.GetTerrainSize().y - 1);
            Vector3 randPos = new Vector3(randX, 0, randZ);
            GoTo(randPos);
        }
    }
}
