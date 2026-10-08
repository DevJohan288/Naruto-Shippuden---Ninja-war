using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    private bool isChangingTurn;
    // =========================================================
    // PERSONAJES
    // =========================================================

    [Header("Battle Characters")]

    [SerializeField]
    private List<BattleCharacter> playerCharacters = new();

    [SerializeField]
    private List<BattleCharacter> enemyCharacters = new();


    // =========================================================
    // SISTEMAS
    // =========================================================

    [Header("Battle Systems")]

    [SerializeField]
    private TurnManager turnManager;

    [SerializeField]
    private ActionSystem actionSystem;

    [SerializeField]
    private TargetSystem targetSystem;


    public TargetSystem TargetSystem =>
        targetSystem;


    public TurnManager TurnManager =>
        turnManager;


    public ActionSystem ActionSystem =>
        actionSystem;


    // =========================================================
    // ESTADO
    // =========================================================

    public BattleState CurrentState { get; private set; }

    public System.Action<BattleState> OnBattleStateChanged;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        if (actionSystem != null)
        {
            actionSystem.OnActionFinished +=
                HandleActionFinished;
        }
    }


    private void OnDisable()
    {
        if (actionSystem != null)
        {
            actionSystem.OnActionFinished -=
                HandleActionFinished;
        }
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    private void Start()
    {
        InitializeBattle();
    }


    // =========================================================
    // INICIAR BATALLA
    // =========================================================

    private void InitializeBattle()
    {
        CurrentState =
            BattleState.Starting;


        // =====================================================
        // JUGADORES
        // =====================================================

        foreach (BattleCharacter character in playerCharacters)
        {
            if (character == null)
                continue;

            character.Initialize(
                Team.Player
            );
        }


        // =====================================================
        // ENEMIGOS
        // =====================================================

        foreach (BattleCharacter character in enemyCharacters)
        {
            if (character == null)
                continue;

            character.Initialize(
                Team.Enemy
            );
        }


        // =====================================================
        // TURNOS
        // =====================================================

        if (turnManager == null)
        {
            Debug.LogError(
                "BattleManager: TurnManager no está asignado."
            );

            return;
        }


        turnManager.Initialize(
            playerCharacters,
            enemyCharacters
        );


        // =====================================================
        // ACTUALIZAR ESTADO
        // =====================================================

        UpdateTurnState();


    }


    // =========================================================
    // ACCIÓN TERMINADA
    // =========================================================

    private void HandleActionFinished()
{
    if (isChangingTurn)
    {
        return;
    }

    if (CheckBattleResult())
        return;


    if (turnManager == null)
        return;


    isChangingTurn = true;


    turnManager.EndTurn();


    UpdateTurnState();


    isChangingTurn = false;
}


    // =========================================================
    // ACTUALIZAR ESTADO
    // =========================================================

    public void UpdateTurnState()
    {
        if (turnManager == null)
            return;


        BattleCharacter character =
            turnManager.CurrentCharacter;


        if (character == null)
            return;


        BattleState newState;


        if (character.Team == Team.Player)
        {
            newState =
                BattleState.PlayerTurn;
        }
        else
        {
            newState =
                BattleState.EnemyTurn;
        }


        CurrentState =
            newState;


        OnBattleStateChanged?.Invoke(
            CurrentState
        );
    }


    // =========================================================
    // COMPROBAR RESULTADO
    // =========================================================

    public bool CheckBattleResult()
    {
        if (CurrentState == BattleState.Victory ||
            CurrentState == BattleState.Defeat)
        {
            return true;
        }


        bool playersAlive =
            playerCharacters.Any(
                character =>
                    character != null &&
                    character.IsAlive
            );


        bool enemiesAlive =
            enemyCharacters.Any(
                character =>
                    character != null &&
                    character.IsAlive
            );


        // =====================================================
        // VICTORIA
        // =====================================================

        if (!enemiesAlive)
        {
            CurrentState =
                BattleState.Victory;


            OnBattleStateChanged?.Invoke(
                CurrentState
            );


            Debug.Log(
                "VICTORIA"
            );


            return true;
        }


        // =====================================================
        // DERROTA
        // =====================================================

        if (!playersAlive)
        {
            CurrentState =
                BattleState.Defeat;


            OnBattleStateChanged?.Invoke(
                CurrentState
            );


            Debug.Log(
                "DERROTA"
            );


            return true;
        }


        return false;
    }


    // =========================================================
    // LISTAS PÚBLICAS
    // =========================================================

    public IReadOnlyList<BattleCharacter> PlayerCharacters =>
        playerCharacters;


    public IReadOnlyList<BattleCharacter> EnemyCharacters =>
        enemyCharacters;
}
