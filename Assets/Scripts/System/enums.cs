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
    AskGroup,
    JoinGroup,
    LeaveGroup,
    //UnitAttacked  //unused for the time being
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

[System.Flags]
public enum LosingCondition
{
    AllUnitsDead = 1,
    FlagsTaken = 2,
}