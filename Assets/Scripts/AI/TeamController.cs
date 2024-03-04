using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] protected TeamName team;
    [SerializeField] private List<UnitGeneral> units = new List<UnitGeneral>();
    //private Dictionary<Guid, UnitGeneral> units = new Dictionary<Guid, UnitGeneral>();
    //private List<UnitMessages> messagesExchange = new List<UnitMessages>();
    private Dictionary<Guid, UnitMessages> messagesExchange = new Dictionary<Guid, UnitMessages>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (!messagesExchange.ContainsKey(unitID))
        {
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
}
