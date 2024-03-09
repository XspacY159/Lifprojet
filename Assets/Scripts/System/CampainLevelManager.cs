using System;
using System.Collections.Generic;
using UnityEngine;

public class CampainLevelManager : LevelManager
{
    [SerializeField] private WinCondition winCondition;

    private Dictionary<WinCondition, TeamController> conditionsMetByTeams = new Dictionary<WinCondition, TeamController>();
 
    private bool winConditionMet;
    private TeamName playerTeam;

    private void OnEnable()
    {
        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
            conditionsMetByTeams.Add(WinCondition.KillAllEnemies, null);
        if (winCondition.HasFlag(WinCondition.CaptureFlags))
            conditionsMetByTeams.Add(WinCondition.CaptureFlags, null);
        if (winCondition.HasFlag(WinCondition.Countdown))
            conditionsMetByTeams.Add(WinCondition.Countdown, null);
    }

    private void Start()
    {
        playerTeam = GameInfoManager.Instance.GetPlayerTeam();
        winConditionMet = false;
    }
    private void FixedUpdate()
    {
        if (UnitManager.Instance == null) return;
        if (UnitManager.Instance.unitsList.Count == 0) return;
        if (winConditionMet) return;

        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
            AllUnitsDead();
        //if (winCondition.HasFlag(WinCondition.CaptureFlags))
        //if (winCondition.HasFlag(WinCondition.Countdown))

        TeamController winner = null;
        foreach (WinCondition condition in conditionsMetByTeams.Keys)
        {
            if (conditionsMetByTeams[condition] == null)
                return;

            if(winner == null)
            {
                winner = conditionsMetByTeams[condition];
                continue;
            }

            if (winner != conditionsMetByTeams[condition])
                return;

            winner = conditionsMetByTeams[condition];
        }

        winConditionMet = true;
        Debug.Log(winner.GetTeamName());
    }

    private void AllUnitsDead()
    {
        TeamController aliveTeam = null;
        int aliveTeamsCount = 0;
        foreach (TeamController team in UnitManager.Instance.GetTeamControllers())
        {
            if (team.GetUnits().Count > 0)
            {
                aliveTeam = team;
                aliveTeamsCount++;
            }
        }
        Debug.Log(aliveTeamsCount);
        if(aliveTeamsCount == 1)
        {
            Debug.Log(aliveTeam.GetTeamName());
            conditionsMetByTeams[WinCondition.KillAllEnemies] = aliveTeam;
        }
        else
        {
            conditionsMetByTeams[WinCondition.KillAllEnemies] = null;
        }
    }
}
