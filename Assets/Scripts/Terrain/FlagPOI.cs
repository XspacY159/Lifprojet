using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlagPOI : POI
{
    [SerializeField] private float captureTime;
    [SerializeField] private float unitCaptureBoost;
    [SerializeField] private float maxCaptureBoost;

    private Dictionary<TeamName, List<UnitGeneral>> capuringUnits = new Dictionary<TeamName, List<UnitGeneral>>();
    private Dictionary<TeamName, float> captureRates = new Dictionary<TeamName, float>();
    private List<TeamName> capturingTeams = new List<TeamName>();

    private void OnEnable()
    {
        interactionEvent += OnInteraction;
    }

    private void OnDisable()
    {
        interactionEvent -= OnInteraction;
    }

    private void Update()
    {
        foreach (TeamName team in capuringUnits.Keys)
        {
            if (capuringUnits[team].Count == 0 && capturingTeams.Contains(team))
            {
                capturingTeams.Remove(team);
            }
        }

        foreach (TeamName team in capuringUnits.Keys)
        {
            float captureDelta = 0;
            if (capuringUnits[team].Count == 0)
            {
                if (captureTime == 0)
                {
                    captureRates[team] = 0;
                    return;
                }

                if (captureRates[team] < 1 || (!capturingTeams.Contains(team) && capturingTeams.Count > 0))
                    captureDelta = -Time.deltaTime / captureTime;
            }
            else
            {
                if (captureTime == 0)
                {
                    captureRates[team] = 1;
                    return;
                }
                float captureBoost = Mathf.Clamp(1 + (capuringUnits[team].Count - 1) * unitCaptureBoost, 0, maxCaptureBoost);
                captureDelta = captureBoost * Time.deltaTime / captureTime;
            }

            if (capturingTeams.Count < 2)
            {
                captureRates[team] += captureDelta;
                captureRates[team] = Mathf.Clamp(captureRates[team], 0, 1);
            }
            Debug.Log(team + " : " + captureRates[team]);
        }
    }

    private void OnInteraction(Transform agent)
    {
        UnitGeneral unit = UnitManager.Instance.GetUnit(agent.gameObject);
        TeamName unitTeam = unit.GetTeam();

        if (!captureRates.ContainsKey(unitTeam))
            captureRates.Add(unitTeam, 0);

        if (!capuringUnits.ContainsKey(unitTeam))
        {
            List<UnitGeneral> units = new List<UnitGeneral> { unit };
            capuringUnits.Add(unitTeam, units);
        }
        else
        {
            capuringUnits[unitTeam].Add(unit);
        }

        if (!capturingTeams.Contains(unitTeam))
            capturingTeams.Add(unitTeam);

        unit.OnStopTryInteract += OnUnitStopTryInteract;
    }

    private void OnUnitStopTryInteract(UnitGeneral unit)
    {
        TeamName unitTeam = unit.GetTeam();

        if (capuringUnits.ContainsKey(unitTeam) && capuringUnits[unitTeam].Contains(unit))
            capuringUnits[unitTeam].Remove(unit);

        unit.OnStopTryInteract -= OnUnitStopTryInteract;
    }

    public float GetCaptureRate()
    {
        return captureRates.Values.Max();
    }

    internal TeamName GetCaptureTeam()
    {
        foreach (TeamName team in captureRates.Keys)
        {
            if (captureRates[team] == 1)
                return team;
        }

        return TeamName.None;
    }
}
