using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMessages
{
    public Guid emitter;
    public MessageObject messageObject;    //specifies the type of response expected
    public Vector3 position;
    public int priority;
    public UnitGeneral targetUnit;
    public POI targetObjective;
    public Guid groupID;

    public UnitMessages (Guid _emitter, MessageObject _messageObject, Vector3 _position,
        int _priority = 0, UnitGeneral _targetUnit = null, POI _targetObjective = null)
    {
        emitter = _emitter;
        messageObject = _messageObject;
        position = _position;
        priority = _priority;
        targetUnit = _targetUnit;
        targetObjective = _targetObjective;
        groupID = Guid.Empty;
    }

    //constructor specific to the group messages
    //required because guid cannot be initialized empty in the declaration
    public UnitMessages(Guid _emitter, MessageObject _messageObject, Vector3 _position, Guid _groupID, int _priority = 0)
    {
        emitter = _emitter;
        messageObject = _messageObject;
        position = _position;
        groupID = _groupID;
        priority = _priority;
    }
}
