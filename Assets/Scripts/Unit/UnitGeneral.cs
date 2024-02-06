using System;
using System.Collections;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    public string Name;
    [SerializeField] protected UnitStats unitStats;  //current stats of the unit, taking into account modifiers

    [SerializeField]
    private UnitType_SO type;

    [SerializeField] protected UnitControls controls;

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);  //hides the selection hihlight mesh
        StartCoroutine(OnEnableDelay());

        unitStats.SetStats(type.baseStats);

        MeshRenderer mR = GetComponent<MeshRenderer>(); //attempt to auto color units according to their type
        mR.sharedMaterial = type.typeMaterial;          //->currently unsuccessful
    }

    private IEnumerator OnEnableDelay()
    {
        yield return new WaitUntil(() => UnitSelectionController.Instance != null);
        UnitSelectionController.Instance.AddUnit(this);
    }

    private void OnDisable()
    {
        UnitSelectionController.Instance.RemoveUnit(this);
    }

    public UnitStats GetStats()
    {
        return unitStats;
    }

    public void GoTo(Vector3 pos)
    {
        controls.GoTo(pos);
    }
}
