using System;
using UnityEngine;

public class RessourcePOI : MonoBehaviour
{
    [SerializeField] private POI poi;
    [SerializeField] private float ressourcesCount;
    [SerializeField] private float maxRessources;
    [SerializeField] private float ressourceProductionRate;

    private Guid poiID = new Guid();

    private void OnEnable()
    {
        poi.interactionEvent += OnInteraction;
        ressourcesCount = 0;
    }

    private void OnDisable()
    {
        poi.interactionEvent -= OnInteraction;
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
        UnitGeneral unit = UnitSelectionController.Instance.unitsList[agent.gameObject];

        ressourcesCount -= unit.CollectRessources(ressourcesCount);
        unit.StopTryInteract();

        Debug.Log(ressourcesCount);
    }
}
