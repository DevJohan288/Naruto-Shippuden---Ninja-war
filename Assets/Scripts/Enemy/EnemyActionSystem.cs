using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyActionSystem : MonoBehaviour
{
    [Header("Battle Systems")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ActionSystem actionSystem;
    [SerializeField] private BattleManager battleManager;

    [Header("Enemy Turn")]
    [SerializeField] private float actionDelay = 0.5f;

    private bool isExecutingEnemyTurn;

    private void OnEnable()
    {
        if (turnManager != null)
            turnManager.OnTurnStarted += HandleTurnStarted;
    }

    private void OnDisable()
    {
        if (turnManager != null)
            turnManager.OnTurnStarted -= HandleTurnStarted;
    }

    private void HandleTurnStarted(BattleCharacter character)
    {
        if (character == null || character.Team != Team.Enemy || isExecutingEnemyTurn)
            return;

        if (battleManager != null &&
            (battleManager.CurrentState == BattleState.Victory ||
             battleManager.CurrentState == BattleState.Defeat))
        {
            return;
        }

        StartCoroutine(ExecuteEnemyTurnRoutine(character));
    }

    private IEnumerator ExecuteEnemyTurnRoutine(BattleCharacter attacker)
    {
        isExecutingEnemyTurn = true;

        if (turnManager == null || actionSystem == null || battleManager == null)
        {
            Debug.LogError("EnemyActionSystem: faltan referencias.", this);
            isExecutingEnemyTurn = false;
            yield break;
        }

        if (turnManager.CurrentCharacter != attacker || !attacker.IsAlive || battleManager.CheckBattleResult())
        {
            isExecutingEnemyTurn = false;
            yield break;
        }

        if (actionDelay > 0f)
            yield return new WaitForSeconds(actionDelay);

        List<BattleCharacter> targets = battleManager.PlayerCharacters
            .Where(character => character != null && character.IsAlive)
            .ToList();

        if (targets.Count == 0)
        {
            isExecutingEnemyTurn = false;
            yield break;
        }

        BattleCharacter target = targets[Random.Range(0, targets.Count)];

        actionSystem.ExecuteBasicAttack(attacker, target);

        const float startTimeout = 2f;
        float elapsed = 0f;
        while (!actionSystem.IsExecutingAction && elapsed < startTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!actionSystem.IsExecutingAction)
        {
            Debug.LogError("EnemyActionSystem: no se pudo iniciar la acción enemiga.", this);
            isExecutingEnemyTurn = false;
            yield break;
        }

        yield return new WaitUntil(() => !actionSystem.IsExecutingAction);

        // ActionSystem notifica el fin de acción antes de que esta corrutina
        // continúe. Si el siguiente turno también es enemigo, el evento llegó
        // mientras este bloqueo seguía activo; lo procesamos ahora.
        isExecutingEnemyTurn = false;
        HandleTurnStarted(turnManager.CurrentCharacter);
    }
}
