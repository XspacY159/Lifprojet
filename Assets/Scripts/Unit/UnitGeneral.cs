using System;
using System.Collections;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    public string Name;
    [SerializeField] private UnitStats baseStats; //base stats of the unit
    protected UnitStats unitStats;                      //current stats of the unit, taking into account modifiers

    [SerializeField] protected UnitControls controls;

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(OnEnableDelay());
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
        return baseStats;
    }

    public void GoTo(Vector3 pos)
    {
        controls.GoTo(pos);
    }
}
