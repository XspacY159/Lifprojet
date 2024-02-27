using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] protected TeamName team;
    [SerializeField] private List<UnitGeneral> units = new List<UnitGeneral>();
    private List<UnitMessages> messagesExchange = new List<UnitMessages>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].setMessageAddress(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendMessageToAll(UnitMessages message)
    {
        for(int i = 0; i < units.Count; i++)
        {
            messagesExchange[i] = message;
        }
    }

    public void sendMessageToUnit(UnitMessages message, int unitID)
    {
        messagesExchange[unitID] = message;
    }
}
