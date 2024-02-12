using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsDisplay : MonoBehaviour
{
    private List<UnitGeneral> selectedUnits = new List<UnitGeneral>();
    private void OnEnable()
    {
        StartCoroutine(OnEnableDelay());
    }

    private IEnumerator OnEnableDelay()
    {
        yield return new WaitUntil(() => UnitSelectionController.Instance != null);
        UnitSelectionController.Instance.changeUnitSelection += OnChangeSelection;
    }

    private void OnDisable()
    {
        UnitSelectionController.Instance.changeUnitSelection -= OnChangeSelection;
    }

    private void OnChangeSelection()
    {
        selectedUnits = UnitSelectionController.Instance.GetSelectedUnits();
        if (selectedUnits.Count == 0)
        {
            return;
        }

        foreach (UnitGeneral unit in selectedUnits)
        {
            Debug.Log(unit.GetStats().attackDamage);
        }
    }
}
