using System;
using System.Collections;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    [SerializeField] protected UnitStats stats;
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
        return stats;
    }

    public void GoTo(Vector3 pos)
    {
        controls.GoTo(pos);
    }
}

[Serializable]
public class UnitStats
{
    public string unitName;
    public int unitHealth;
    public int unitMoveSpeed;
    public int unitAttackDamage;
    public int unitAttackRange;
}
