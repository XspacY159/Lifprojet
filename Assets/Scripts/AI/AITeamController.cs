using System;
using System.Collections.Generic;
using UnityEngine;

public class AITeamController : TeamController
{
    //stocks the messages of the team, guid refers to units ID
    private Dictionary<Guid, UnitMessages> messagesExchange = new Dictionary<Guid, UnitMessages>();
    //stocks the groups of the team, guid refers to groups ID
    private Dictionary<Guid, GroupController> unitsGroups = new Dictionary<Guid, GroupController>();
    //stocks the action's priorities of each units in the team
    private Dictionary<UnitGeneral, Dictionary<AIState, int>> unitActionPriorities = new Dictionary<UnitGeneral, Dictionary<AIState, int>>();
    [SerializeField] private GameObject groupTreePrefab;
    private Guid teamId = Guid.NewGuid();

    private void OnEnable()
    {
        foreach (UnitGeneral unit in units)
        {
            Dictionary<AIState, int> priorities = new Dictionary<AIState, int>
            {
                { AIState.Aggressive, 0 },
                { AIState.Defensive, 0 },
                { AIState.FollowObjective, 0 }
            };

            priorities[unit.baseState] = unit.currentActionPriority;
            unitActionPriorities.Add(unit, priorities);
        }
    }

    private void Update()
    {
        if(!TimerManager.StartTimer(0.25f, "AI Team Priorities Refresh" + teamId))
        {
            foreach (UnitGeneral unit in unitActionPriorities.Keys)
            {

            }
        }
    }

    public override void RemoveUnit(UnitGeneral unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
            foreach (GroupController unitGroup in unitsGroups.Values)
            {
                unitGroup.RemoveUnit(unit);
            }
        }
    }

    public void SendMessageToAll(UnitMessages message)
    {
        foreach (UnitGeneral unit in units)
        {
            if (!messagesExchange.ContainsKey(unit.GetUnitID()))
            {
                messagesExchange.Add(unit.GetUnitID(), message);
            }
        }
    }

    public void SendMessageToUnit(UnitMessages message, Guid unitID)
    {
        if ((!messagesExchange.ContainsKey(unitID)) || (message.priority > 4))
        {
            RemoveMessage(unitID);
            messagesExchange.Add(unitID, message);
        }
    }

    public UnitMessages ReadMessage(Guid receivingUnitID)
    {
        if (messagesExchange.ContainsKey(receivingUnitID))
        {
            if (messagesExchange[receivingUnitID].emitter != receivingUnitID)
            {
                UnitMessages tmp = messagesExchange[receivingUnitID];
                RemoveMessage(receivingUnitID);
                return tmp;
            }
            else    //the receiver is the sender, so it's not concerned by receiving the message
            {
                RemoveMessage(receivingUnitID);
                return null;
            }
        }
        else
            return null;
    }

    public void RemoveMessage(Guid unitID)
    {
        if (messagesExchange.ContainsKey(unitID))
            messagesExchange.Remove(unitID);
    }

    public Guid CreateGroup()
    {
        GroupController newGroup = new GroupController(Instantiate(groupTreePrefab), this);

        unitsGroups.Add(newGroup.GetID(), newGroup);
        return newGroup.GetID();
    }

    public void DeleteGroup(Guid groupID)
    {
        Destroy(unitsGroups[groupID].GetGroupTree());
    }

    public void JoinGroup(UnitGeneral joiningUnit, Guid groupToJoin)
    {
        unitsGroups[groupToJoin].AddUnit(joiningUnit);
    }
}
