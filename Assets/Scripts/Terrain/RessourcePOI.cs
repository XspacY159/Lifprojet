using System;
using UnityEngine;

public class RessourcePOI : POI
{
    [SerializeField] private float ressourcesCount;
    [SerializeField] private float maxRessources;
    [SerializeField] private float ressourceProductionRate;

    private Guid poiID = new Guid();

    private void OnEnable()
    {
        interactionEvent += OnInteraction;
        ressourcesCount = 0;
    }

    private void OnDisable()
    {
        interactionEvent -= OnInteraction;
    }

    private void Update()
    {
        if (ressourcesCount >= maxRessources)
        {
            TimerManager.Cancel("ressourcesProd" + poiID);
            return;
        }

        if (!TimerManager.StartTimer(1f, "ressourcesProd" + poiID))
        {
            ressourcesCount += ressourceProductionRate;
            ressourcesCount = Mathf.Clamp(ressourcesCount, 0, maxRessources);
        }
    }

    private void OnInteraction(Transform agent)
    {
        UnitGeneral unit = UnitManager.Instance.GetUnit(agent.gameObject);

        ressourcesCount -= unit.CollectRessources(ressourcesCount);
        unit.StopTryInteract();
    }
}
