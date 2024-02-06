using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitType", menuName = "LifProjet/NewUnitType")]
public class UnitType_SO : ScriptableObject
{
    [Header("Unit base stats")]

    public UnitStats baseStats;

    [Header("Visual properties")]
    public Material typeMaterial;   //temporary material to differentiate unit types => to replace with mesh
}
