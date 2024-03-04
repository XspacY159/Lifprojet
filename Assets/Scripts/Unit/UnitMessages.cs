using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMessages
{
    public Guid emitter;
    public MessageObject messageObject;    //specifies the type of response expected
    public Vector3 position;
    public int urgency;
    public UnitGeneral targetUnit;
    
    //public Dictionary<string, float> messageContents;
        //position x
        //position y
        //position z
        //urgency
        //health of sender?
        //number of ennemies?

    public UnitMessages (Guid _emitter, MessageObject _messageObject, Vector3 _position, UnitGeneral _targetUnit = null, int _urgency = 0)
    {
        emitter = _emitter;
        messageObject = _messageObject;
        position = _position;
        targetUnit = _targetUnit;
        urgency = _urgency;
        
        /*messageContents.Add("x_position", pos.x);
        messageContents.Add("y_position", pos.y);
        messageContents.Add("z_position", pos.z);
        messageContents.Add("urgency", urgency);*/
    }
}
