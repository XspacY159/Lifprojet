using System;
using System.Collections.Generic;
using UnityEngine;

public class AITeamController : TeamController
{
    //stocks the messages of the team, guid refers to units ID
    private Dictionary<Guid, UnitMessages> messagesExchange = new Dictionary<Guid, UnitMessages>();
    //stocks the groups of the team, guid refers to groups ID
    private Dictionary<Guid, GroupController> unitsGroups = new Dictionary<Guid, GroupController>();
    [SerializeField] private GameObject groupTreePrefab;

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
        GroupController newGroup = new GroupController(Instantiate(groupTreePrefab));

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
