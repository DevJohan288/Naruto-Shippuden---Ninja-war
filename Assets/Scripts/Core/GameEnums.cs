public enum Team
{
    Player,
    Enemy
}

public enum BattleState
{
    None,
    Starting,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public enum ActionType
{
    None,
    Attack,
    Jutsu,
    Change
}

public enum TargetType
{
    SingleEnemy,
    SingleAlly,
    AllEnemies,
    AllAllies,
    Self
}