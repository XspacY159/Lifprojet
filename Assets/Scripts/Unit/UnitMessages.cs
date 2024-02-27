using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMessages : MonoBehaviour
{
    private int emitter;
    private MessageObject messageObject;    //specifies the type of response expected
    private Dictionary<string, float> messageContents;
        //position x
        //position y
        //urgency
        //health of sender?
        //number of ennemies?

    public UnitMessages (int _emitter, MessageObject _messageObject, Vector3 pos, float urgency = 0)
    {
        emitter = _emitter;
        messageObject = _messageObject;
        messageContents.Add("x_position", pos.x);
        messageContents.Add("y_position", pos.y);
        messageContents.Add("z_position", pos.z);
        messageContents.Add("urgency", urgency);
    }
}
