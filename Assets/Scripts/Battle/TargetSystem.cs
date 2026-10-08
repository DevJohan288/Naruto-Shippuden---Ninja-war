using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    public Action<BattleCharacter> OnTargetSelected;

    private BattleCharacter selectedTarget;
    private BattleCharacter currentAttacker;

    public bool IsSelectingTarget { get; private set; }
    public BattleCharacter SelectedTarget => selectedTarget;
    public BattleCharacter CurrentAttacker => currentAttacker;

    public void StartTargetSelection(BattleCharacter attacker, List<BattleCharacter> enemies)
    {
        if (attacker == null || turnManager == null || IsSelectingTarget)
            return;

        if (turnManager.CurrentCharacter != attacker || attacker.Team != Team.Player || !attacker.IsAlive)
            return;

        ClearTarget();
        currentAttacker = attacker;
        IsSelectingTarget = true;

        if (GetValidEnemyTargets(attacker, enemies).Count == 0)
            ClearTarget();
    }

    public List<BattleCharacter> GetValidEnemyTargets(BattleCharacter attacker, List<BattleCharacter> enemies)
    {
        if (attacker == null || enemies == null)
            return new List<BattleCharacter>();

        return enemies
            .Where(enemy => enemy != null && enemy.IsAlive && enemy.Team != attacker.Team)
            .ToList();
    }

    public void SelectTarget(BattleCharacter target)
    {
        if (!IsSelectingTarget || target == null || currentAttacker == null ||
            turnManager == null || turnManager.CurrentCharacter != currentAttacker ||
            !currentAttacker.IsAlive || !target.IsAlive ||
            target.Team == currentAttacker.Team)
            return;

        ClearPreviousSelection();
        selectedTarget = target;
        target.GetComponent<TargetSelectable>()?.SetSelected(true);

        IsSelectingTarget = false;
        OnTargetSelected?.Invoke(selectedTarget);
    }

    public void ClearTarget()
    {
        ClearPreviousSelection();
        selectedTarget = null;
        currentAttacker = null;
        IsSelectingTarget = false;
    }

    private void ClearPreviousSelection()
    {
        if (selectedTarget != null)
            selectedTarget.GetComponent<TargetSelectable>()?.SetSelected(false);
    }
}
