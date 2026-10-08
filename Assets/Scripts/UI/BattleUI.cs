using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Battle UI")]

    [SerializeField]
    private GameObject attackButton;


    [Header("Battle Systems")]

    [SerializeField]
    private TurnManager turnManager;

    [SerializeField]
    private TargetSystem targetSystem;

    [SerializeField]
    private BattleManager battleManager;

    [SerializeField]
    private ActionSystem actionSystem;


    [Header("Battle Result")]

    [SerializeField]
    private GameObject battleResultPanel;

    [SerializeField]
    private Text resultText;


    private BattleCharacter currentCharacter;


    // =========================================================
    // INICIO
    // =========================================================

    private void Start()
    {
        HidePlayerActions();


        if (battleResultPanel != null)
        {
            battleResultPanel.SetActive(false);
        }


        if (turnManager != null)
        {
            turnManager.OnTurnStarted +=
                HandleTurnStarted;
        }


        if (targetSystem != null)
        {
            targetSystem.OnTargetSelected +=
                HandleTargetSelected;
        }


        if (battleManager != null)
        {
            battleManager.OnBattleStateChanged +=
                HandleBattleStateChanged;
        }


        // -----------------------------------------------------
        // Sincronizar turno actual
        // -----------------------------------------------------

        if (turnManager != null &&
            turnManager.CurrentCharacter != null)
        {
            HandleTurnStarted(
                turnManager.CurrentCharacter
            );
        }
    }


    // =========================================================
    // DESTRUIR
    // =========================================================

    private void OnDestroy()
    {
        if (turnManager != null)
        {
            turnManager.OnTurnStarted -=
                HandleTurnStarted;
        }


        if (targetSystem != null)
        {
            targetSystem.OnTargetSelected -=
                HandleTargetSelected;
        }


        if (battleManager != null)
        {
            battleManager.OnBattleStateChanged -=
                HandleBattleStateChanged;
        }
    }


    // =========================================================
    // NUEVO TURNO
    // =========================================================

    private void HandleTurnStarted(
        BattleCharacter character)
    {
        if (character == null)
            return;


        currentCharacter =
            character;


        // Limpiar cualquier selección anterior.
        if (targetSystem != null)
        {
            targetSystem.ClearTarget();
        }


        if (character.Team == Team.Player)
        {
            ShowPlayerActions();
        }
        else
        {
            HidePlayerActions();
        }
    }


    // =========================================================
    // OBJETIVO SELECCIONADO
    // =========================================================

    private void HandleTargetSelected(
        BattleCharacter target)
    {
        if (target == null)
            return;


        if (actionSystem == null ||
            turnManager == null)
            return;


        if (actionSystem.IsExecutingAction)
            return;


        BattleCharacter attacker =
            turnManager.CurrentCharacter;


        if (attacker == null)
            return;


        if (attacker != currentCharacter)
            return;


        if (attacker.Team != Team.Player)
            return;


        if (!attacker.IsAlive)
            return;


        if (!target.IsAlive)
            return;


        actionSystem.ExecuteBasicAttack(
            attacker,
            target
        );


        // La selección visual ya no es necesaria.
        if (targetSystem != null)
        {
            targetSystem.ClearTarget();
        }
    }


    // =========================================================
    // ESTADO DE BATALLA
    // =========================================================

    private void HandleBattleStateChanged(
        BattleState state)
    {
        if (state == BattleState.Victory)
        {
            ShowBattleResult(
                "VICTORIA"
            );

            return;
        }


        if (state == BattleState.Defeat)
        {
            ShowBattleResult(
                "DERROTA"
            );
        }
    }


    // =========================================================
    // MOSTRAR RESULTADO
    // =========================================================

    private void ShowBattleResult(
        string message)
    {
        HidePlayerActions();


        if (battleResultPanel != null)
        {
            battleResultPanel.SetActive(true);
        }


        if (resultText != null)
        {
            resultText.text =
                message;
        }


    }


    // =========================================================
    // MOSTRAR ACCIONES
    // =========================================================

    private void ShowPlayerActions()
{
    if (actionSystem != null &&
        actionSystem.IsExecutingAction)
    {
        return;
    }

    if (battleManager != null &&
        (battleManager.CurrentState == BattleState.Victory ||
         battleManager.CurrentState == BattleState.Defeat))
    {
        return;
    }

    if (attackButton != null)
    {
        attackButton.SetActive(true);
    }
}


    // =========================================================
    // OCULTAR ACCIONES
    // =========================================================

    private void HidePlayerActions()
    {
        if (attackButton != null)
        {
            attackButton.SetActive(false);
        }
    }


    // =========================================================
    // BOTÓN ATAQUE
    // =========================================================

    public void OnAttackPressed()
    {
        if (currentCharacter == null)
            return;


        if (turnManager == null ||
            targetSystem == null ||
            battleManager == null ||
            actionSystem == null)
        {
            return;
        }


        // -----------------------------------------------------
        // No permitir durante una acción.
        // -----------------------------------------------------

        if (actionSystem.IsExecutingAction)
        {
            return;
        }


        // -----------------------------------------------------
        // No permitir si ya estamos seleccionando.
        // -----------------------------------------------------

        if (targetSystem.IsSelectingTarget)
        {
            return;
        }


        // -----------------------------------------------------
        // Debe ser el personaje actual.
        // -----------------------------------------------------

        if (turnManager.CurrentCharacter !=
            currentCharacter)
        {
            return;
        }


        // -----------------------------------------------------
        // Debe pertenecer al jugador.
        // -----------------------------------------------------

        if (currentCharacter.Team != Team.Player)
        {
            return;
        }


        // -----------------------------------------------------
        // Debe estar vivo.
        // -----------------------------------------------------

        if (!currentCharacter.IsAlive)
        {
            return;
        }


        // -----------------------------------------------------
        // Iniciar selección.
        // -----------------------------------------------------

        System.Collections.Generic.List<BattleCharacter>
            enemies =
                new System.Collections.Generic.List<BattleCharacter>(
                    battleManager.EnemyCharacters
                );


        targetSystem.StartTargetSelection(
            currentCharacter,
            enemies
        );


        HidePlayerActions();


    }
}
