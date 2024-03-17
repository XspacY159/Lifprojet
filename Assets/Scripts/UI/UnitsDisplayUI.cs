using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsDisplayUI : MonoBehaviour
{
    [SerializeField] private UnitInfoDisplay infoDisplayPrefab;
    [SerializeField] private Transform unitInfoContainer;

    private List<UnitGeneral> selectedUnits = new List<UnitGeneral>();
    private List<UnitInfoDisplay> unitsInfoDisplays = new List<UnitInfoDisplay>();

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

        while (unitsInfoDisplays.Count > 0)
        {
            Destroy(unitsInfoDisplays[0].gameObject);
            unitsInfoDisplays.RemoveAt(0);
        }

        if (selectedUnits.Count == 0)
        {
            return;
        }

        foreach (UnitGeneral unit in selectedUnits)
        {
            UnitInfoDisplay unitInfoDisplay = Instantiate(infoDisplayPrefab, unitInfoContainer, false);

            unitInfoDisplay.unitNameText.text = unit.unitName;
            unitInfoDisplay.unitHPText.text = "HP : " + unit.GetStats().maxHealth;
            unitInfoDisplay.unitAttackDamageText.text = "Dmaage : " + unit.GetStats().attackDamage;

            unitsInfoDisplays.Add(unitInfoDisplay);
        }
    }
}
