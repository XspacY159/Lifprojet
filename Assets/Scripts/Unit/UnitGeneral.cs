using System;
using System.Collections;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    public string unitName;
    [SerializeField] protected TeamName team;
    [SerializeField] protected UnitStats unitStats;  //current stats of the unit, taking into account modifiers
    [SerializeField] protected UnitType_SO type;

    [SerializeField] protected UnitHealth health;
    [SerializeField] protected UnitMovements controls;
    [SerializeField] protected UnitCombat unitCombat;

    [SerializeField] private float ressources;

    public event Action<UnitGeneral> OnTryInteract;
    public event Action<UnitGeneral> OnStopTryInteract;

    public AIState baseState;
    public int currentActionPriority;

    private POI poiToInteract;
    private Guid unitID = System.Guid.NewGuid();
    [SerializeField] private LayerMask unitsLayer;  //used to check collisions only with other units

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);  //hides the selection hihlight mesh
        StartCoroutine(OnEnableDelay());

        unitStats.SetStats(type.baseStats);
        currentActionPriority = 0;

        MeshRenderer mR = GetComponent<MeshRenderer>(); //attempt to auto color units according to their type
        mR.sharedMaterial = type.typeMaterial;          //->currently unsuccessful
    }

    private IEnumerator OnEnableDelay()
    {
        yield return new WaitUntil(() => UnitManager.Instance != null);
        UnitManager.Instance.AddUnit(this);
    }

    private void OnDisable()
    {
        UnitSelectionController.Instance.Deselect(this);
        UnitManager.Instance.RemoveUnit(this);
    }

    private void Update()
    {
        if (poiToInteract != null)
        {
            if (!TimerManager.StartTimer(0.25f, "Unit Interaction Try" + unitID))
            {
                if (poiToInteract.IsInRange(transform))
                    poiToInteract.Interact(transform);
            }
        }
        else
            TimerManager.Cancel("Unit Interaction Try" + unitID);
    }

    public Guid GetUnitID() { return unitID; }

    public UnitStats GetStats()
    {
        return unitStats;
    }

    public TeamName GetTeam()
    {
        return team;
    }

    public void TryInteract(POI poi)
    {
        if (poi == null) return;
        poiToInteract = poi;

        OnTryInteract?.Invoke(this);
    }

    public void StopTryInteract()
    {
        if (poiToInteract == null) return;
        poiToInteract.transform.GetChild(0).gameObject.SetActive(false);
        poiToInteract = null;
        OnStopTryInteract?.Invoke(this);
    }

    public void GoTo(Vector3 pos)
    {
        controls.GoTo(pos);
    }

    public void StopGoTo()
    {
        controls.StopGoTo();
    }

    public float CollectRessources(float _ressources)
    {
        if (ressources >= unitStats.maxRessources)
            return 0;

        float diff = unitStats.maxRessources - ressources;
        ressources += _ressources;

        if (ressources >= unitStats.maxRessources)
        {
            ressources = Mathf.Clamp(ressources, 0, unitStats.maxRessources);
            return diff;
        }

        return _ressources;
    }

    public void AttackUnit(UnitGeneral unitToAttack)
    {
        if (unitToAttack.team == this.team) return;

        unitCombat.SetUnitToAttack(unitToAttack);
    }

    public void StopAttackUnit()
    {
        unitCombat.SetUnitToAttack(null);
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    public Collider[] UnitsInRange()
    {
        return Physics.OverlapSphere(transform.position, unitStats.attackRange, unitsLayer);
    }
}
