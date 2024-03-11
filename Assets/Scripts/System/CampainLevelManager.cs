using System.Collections.Generic;
using UnityEngine;

public class CampainLevelManager : LevelManager
{
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private List<FlagPOI> flagPOIList = new List<FlagPOI>();
    [SerializeField] private float countdown;

    private Dictionary<WinCondition, TeamController> conditionsMetByTeams = new Dictionary<WinCondition, TeamController>();

    private bool winConditionMet;
    private TeamController playerTeam;
    private TeamController winner;

    private void OnEnable()
    {
        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
            conditionsMetByTeams.Add(WinCondition.KillAllEnemies, null);
        if (winCondition.HasFlag(WinCondition.CaptureFlags))
            conditionsMetByTeams.Add(WinCondition.CaptureFlags, null);
        if (winCondition.HasFlag(WinCondition.Countdown))
            TimerManager.StartTimer(countdown, "Level Win Countdown");
    }

    protected override void LevelStart()
    {
        foreach (TeamController team in UnitManager.Instance.GetTeamControllers())
        {
            if (team.GetTeamName() == GameInfoManager.Instance.GetPlayerTeam())
            {
                playerTeam = team;
                break;
            }
        }
        winConditionMet = false;
    }

    private void FixedUpdate()
    {
        if (UnitManager.Instance == null) return;
        if (UnitManager.Instance.unitsList.Count == 0) return;
        if (winConditionMet) return;

        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
            AllUnitsDead();
        if (winCondition.HasFlag(WinCondition.CaptureFlags))
            CapturedFlags();
        if (winCondition.HasFlag(WinCondition.Countdown))
        {
            if (TimerManager.IsCounting("Level Win Countdown"))
                return;
        }

        winner = playerTeam;
        foreach (WinCondition condition in conditionsMetByTeams.Keys)
        {
            if (conditionsMetByTeams[condition] == null)
                return;

            if (winner != null && winner != conditionsMetByTeams[condition])
            {
                return;
            }

            winner = conditionsMetByTeams[condition];
        }

        winConditionMet = true;
        Debug.Log(winner.GetTeamName());
    }

    private void CapturedFlags()
    {
        TeamName captureTeam = TeamName.None;
        foreach (FlagPOI flag in flagPOIList)
        {
            TeamName flagCapturingTeam = flag.GetCaptureTeam();
            if (flagCapturingTeam == TeamName.None || flag.GetCaptureRate() < 1)
            {
                conditionsMetByTeams[WinCondition.CaptureFlags] = null;
                return;
            }

            if (captureTeam != TeamName.None && flag.GetCaptureTeam() != captureTeam)
            {
                conditionsMetByTeams[WinCondition.CaptureFlags] = null;
                return;
            }

            captureTeam = flag.GetCaptureTeam();
        }

        foreach (TeamController team in UnitManager.Instance.GetTeamControllers())
        {
            if (team.GetTeamName() == captureTeam)
            {
                conditionsMetByTeams[WinCondition.CaptureFlags] = team;
                return;
            }
        }
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
        if (aliveTeamsCount == 1)
        {
            conditionsMetByTeams[WinCondition.KillAllEnemies] = aliveTeam;
        }
        else
        {
            conditionsMetByTeams[WinCondition.KillAllEnemies] = null;
        }
    }
}
