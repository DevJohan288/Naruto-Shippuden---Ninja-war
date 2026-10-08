using System;
using System.Collections;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public Action OnActionFinished;

    [SerializeField] private MovementSystem movementSystem;

    private bool isExecutingAction;
    private bool attackHitReceived;
    private bool attackFinishedReceived;
    private BattleCharacter currentAttacker;
    private BattleCharacter currentTarget;

    public bool IsExecutingAction => isExecutingAction;

    public void ExecuteBasicAttack(BattleCharacter attacker, BattleCharacter target)
    {
        if (isExecutingAction || attacker == null || target == null ||
            !attacker.IsAlive || !target.IsAlive)
            return;

        if (movementSystem == null)
        {
            Debug.LogError("ActionSystem: falta MovementSystem.", this);
            return;
        }

        StartCoroutine(ExecuteBasicAttackRoutine(attacker, target));
    }

    private IEnumerator ExecuteBasicAttackRoutine(BattleCharacter attacker, BattleCharacter target)
    {
        isExecutingAction = true;
        currentAttacker = attacker;
        currentTarget = target;

        CharacterAnimationController animationController = attacker.AnimationController;
        if (animationController == null)
        {
            Debug.LogError($"{attacker.name}: falta CharacterAnimationController.", attacker);
            FinishAction();
            yield break;
        }

        attackHitReceived = false;
        attackFinishedReceived = false;
        animationController.OnAttackHit += HandleAttackHit;
        animationController.OnAttackFinished += HandleAttackFinished;

        yield return movementSystem.MoveToAttackPosition(attacker, target);
        animationController.PlayAttack();

        yield return new WaitUntil(() => attackHitReceived);
        bool battleFinished = BattleManager.Instance != null && BattleManager.Instance.CheckBattleResult();

        yield return new WaitUntil(() => attackFinishedReceived);
        RemoveAnimationEvents(animationController);

        if (!battleFinished)
        {
            attacker.ReturnToBattlePosition();
            attacker.RestoreBattleOrientation();
            animationController.PlayIdle();
        }

        FinishAction();
    }

    private void HandleAttackHit()
    {
        if (!isExecutingAction || attackHitReceived)
            return;

        attackHitReceived = true;
        Attack(currentAttacker, currentTarget);
    }

    private void HandleAttackFinished()
    {
        if (isExecutingAction)
            attackFinishedReceived = true;
    }

    public void Attack(BattleCharacter attacker, BattleCharacter target)
    {
        if (attacker == null || target == null || !attacker.IsAlive || !target.IsAlive)
            return;

        target.TakeDamage(attacker.Data.attack);
        attacker.GainChakra(attacker.Data.chakraGainOnBasicAttack);
    }

    private void RemoveAnimationEvents(CharacterAnimationController animationController)
    {
        animationController.OnAttackHit -= HandleAttackHit;
        animationController.OnAttackFinished -= HandleAttackFinished;
    }

    private void FinishAction()
    {
        isExecutingAction = false;
        currentAttacker = null;
        currentTarget = null;
        OnActionFinished?.Invoke();
    }
}
