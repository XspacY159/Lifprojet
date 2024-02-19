using System;
using System.Collections;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    public string unitName;
    [SerializeField] protected Team team;
    [SerializeField] protected UnitStats unitStats;  //current stats of the unit, taking into account modifiers
    [SerializeField] private UnitType_SO type;
    [SerializeField] protected UnitControls controls;

    [SerializeField] private float ressources;

    private POI poiToInteract;
    private Guid timerID = new Guid();

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

    private void Update()
    {
        if (poiToInteract != null)
        {
            if (!TimerManager.StartTimer(0.25f, "Unit Interaction Try" + timerID))
            {
                if (poiToInteract.IsInRange(transform))
                    poiToInteract.Interact(transform);
            }
        }
        else
            TimerManager.Cancel("Unit Interaction Try" + timerID);
    }

    public UnitStats GetStats()
    {
        return unitStats;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void TryInteract(POI poi)
    {
        if (poi == null) return;
        poiToInteract = poi;
        poiToInteract.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void StopTryInteract()
    {
        if (poiToInteract == null) return;
        poiToInteract.transform.GetChild(0).gameObject.SetActive(false);
        poiToInteract = null;
    }

    public void GoTo(Vector3 pos)
    {
        controls.GoTo(pos);
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
}
