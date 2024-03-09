public enum TeamName
{
    Draneds,
    Wasalis,
    Mercenaries,
    Moonwolves,
    None
}

public enum MessageObject
{
    GoToObjective,
    AttackTarget,
    DefendPosition,
    UnitAttacked
}

public enum AIState
{
    Aggressive,
    Defensive,
    FollowObjective
}

[System.Flags]
public enum WinCondition
{
    CaptureFlags = 1,
    Countdown = 2,
    KillAllEnemies = 4,
}