using System.Collections.Generic;
using UnityEngine;

public class FlagPOI : POI
{
    [SerializeField] private float captureTime;
    [SerializeField] private float unitCaptureBoost;
    [SerializeField] private float maxCaptureBoost;
    [SerializeField] private float captureRate = 0;
    private TeamName currentCapturingTeam;
    private List<UnitGeneral> capuringUnits = new List<UnitGeneral>();

    private void OnEnable()
    {
        currentCapturingTeam = TeamName.None;
        interactionEvent += OnInteraction;
    }

    private void OnDisable()
    {
        interactionEvent -= OnInteraction;
    }

    private void Update()
    {
        float captureDelta;
        if (capuringUnits.Count == 0)
        {
            currentCapturingTeam = TeamName.None;
            if (captureTime == 0)
            {
                captureRate = 0;
                return;
            }
            captureDelta = - Time.deltaTime / captureTime;
        }
        else
        {
            if (captureTime == 0)
            {
                captureRate = 1;
                return;
            }
            float captureBoost = Mathf.Clamp(1 + (capuringUnits.Count - 1) * unitCaptureBoost, 0, maxCaptureBoost);
            captureDelta = captureBoost * Time.deltaTime / captureTime;
        }

        captureRate += captureDelta;
        captureRate = Mathf.Clamp(captureRate, 0, 1);
    }

    private void OnInteraction(Transform agent)
    {
        UnitGeneral unit = UnitManager.Instance.GetUnit(agent.gameObject);

        if (currentCapturingTeam != TeamName.None && unit.GetTeam() != currentCapturingTeam)
            return;

        if (!capuringUnits.Contains(unit))
            capuringUnits.Add(unit);
        currentCapturingTeam = unit.GetTeam();
        unit.OnStopTryInteract += OnUnitStopTryInteract;
    }

    private void OnUnitStopTryInteract(UnitGeneral unit)
    {
        if (capuringUnits.Contains(unit))
            capuringUnits.Remove(unit);
        unit.OnStopTryInteract -= OnUnitStopTryInteract;
    }

    public float GetCaptureRate()
    {
        return captureRate;
    }

    public TeamName GetCapturingTeam()
    {
        return currentCapturingTeam;
    }
}
