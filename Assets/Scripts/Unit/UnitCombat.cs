using Unity.VisualScripting;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    [SerializeField] private UnitGeneral unit;

    private UnitGeneral targetUnit;

    private void Update()
    {
        if (targetUnit == null)
        {
            TimerManager.Cancel("Unit Attack" + unit.GetUnitID());
            return;
        }

        if(Vector3.Distance(transform.position, targetUnit.transform.position) <= unit.GetStats().attackRange)
        {
            unit.StopGoTo();
            float cooldown = unit.GetStats().attackCooldown;
            if (!TimerManager.StartTimer(cooldown, "Unit Attack" + unit.GetUnitID()))
            {
                targetUnit.TakeDamage(unit.GetStats().attackDamage);
            }
        }
    }

    public void SetUnitToAttack(UnitGeneral unitToAttack)
    {
        targetUnit = unitToAttack;
        if(unitToAttack == null) return;
        unit.GoTo(targetUnit.transform.position);
    }
}
