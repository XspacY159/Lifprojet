using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CampainLevelManager : LevelManager
{
    [Header("Conditions Parameters")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private DefeatCondition defeatCondition;
    [SerializeField] private List<FlagPOI> flagPOIList = new List<FlagPOI>();
    [SerializeField] private float countdown;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI capturedFlagsText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;

    [Header("UIs")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

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

        enemiesRemainingText.gameObject.SetActive(winCondition.HasFlag(WinCondition.KillAllEnemies));
        capturedFlagsText.gameObject.SetActive(winCondition.HasFlag(WinCondition.CaptureFlags));
        timeRemainingText.gameObject.SetActive(winCondition.HasFlag(WinCondition.Countdown));
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
        CheckVictoryCondition();
        CheckDefeatCondition();
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
        {
            int unitCount = 0;
            foreach (TeamController team in UnitManager.Instance.GetTeamControllers())
            {
                if (team == playerTeam) continue;
                unitCount += team.GetUnits().Count;
            }

            enemiesRemainingText.text = "Enemies Remaining :\r\n" + unitCount;

            if (unitCount == 0) enemiesRemainingText.color = Color.green;
        }

        if (winCondition.HasFlag(WinCondition.CaptureFlags))
        {
            int capturedFlagsCount = 0;
            foreach (FlagPOI flag in flagPOIList)
            {
                TeamName flagCapturingTeam = flag.GetCaptureTeam();
                if (flagCapturingTeam != playerTeam.GetTeamName() || flag.GetCaptureRate() < 1)
                {
                    conditionsMetByTeams[WinCondition.CaptureFlags] = null;
                    continue;
                }
                capturedFlagsCount++;
            }

            capturedFlagsText.text = "Captured Flag :\r\n" +
                capturedFlagsCount + "/" + flagPOIList.Count;

            if (capturedFlagsCount == flagPOIList.Count) capturedFlagsText.color = Color.green;
        }

        if (winCondition.HasFlag(WinCondition.Countdown))
        {
            if (!TimerManager.IsCounting("Level Win Countdown"))
            {
                timeRemainingText.text = "Time To Stay Alive :\r\n00::00";
                timeRemainingText.color = Color.green;
                return;
            }

            float currentCountdownTime = TimerManager.GetTimer("Level Win Countdown").counter;

            int secondes = Mathf.CeilToInt(currentCountdownTime) % 60;
            int minutes = Mathf.CeilToInt(currentCountdownTime) / 60;

            timeRemainingText.text = "Time To Stay Alive :\r\n";

            if (minutes < 10)
                timeRemainingText.text += "0";
            timeRemainingText.text += minutes + ":";

            if (secondes < 10)
                timeRemainingText.text += "0";
            timeRemainingText.text += secondes;
        }
    }

    private void CheckDefeatCondition()
    {
        if(defeatCondition.HasFlag(DefeatCondition.AllUnitsDead) && playerTeam.GetUnits().Count == 0)
            loseScreen.SetActive(true);

        if (defeatCondition.HasFlag(DefeatCondition.FlagsTaken) && CapturedFlags() != playerTeam)
            loseScreen.SetActive(true);
    }

    private void CheckVictoryCondition()
    {
        if (UnitManager.Instance == null) return;
        if (UnitManager.Instance.unitsList.Count == 0) return;
        if (winConditionMet) return;

        if (winCondition.HasFlag(WinCondition.KillAllEnemies))
            conditionsMetByTeams[WinCondition.KillAllEnemies] = AllUnitsDead();
        if (winCondition.HasFlag(WinCondition.CaptureFlags))
            conditionsMetByTeams[WinCondition.CaptureFlags] = CapturedFlags();
        if (winCondition.HasFlag(WinCondition.Countdown))
        {
            if (TimerManager.IsCounting("Level Win Countdown"))
                return;
        }

        winner = null;
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
        if (winner == null) winner = playerTeam;

        Debug.Log("Winning team is : " + winner.GetTeamName());
        if(winner == playerTeam)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
        }
    }

    private TeamController CapturedFlags()
    {
        TeamName captureTeam = TeamName.None;
        foreach (FlagPOI flag in flagPOIList)
        {
            TeamName flagCapturingTeam = flag.GetCaptureTeam();
            if (flagCapturingTeam == TeamName.None || flag.GetCaptureRate() < 1)
            {
                return null;
            }

            if (captureTeam != TeamName.None && flag.GetCaptureTeam() != captureTeam)
            {
                return null;
            }

            captureTeam = flag.GetCaptureTeam();
        }

        foreach (TeamController team in UnitManager.Instance.GetTeamControllers())
        {
            if (team.GetTeamName() == captureTeam)
            {
                return team;
            }
        }

        return null;
    }

    private TeamController AllUnitsDead()
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
            return aliveTeam;
        }
        else
        {
            return null;
        }
    }
}
