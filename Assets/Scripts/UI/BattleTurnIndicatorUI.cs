using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presenta el número y el equipo del turno actual.
/// La lógica de orden de turnos permanece en TurnManager.
/// </summary>
public class BattleTurnIndicatorUI : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Text turnNumberText;
    [SerializeField] private Text turnTeamText;

    [Header("Team Colors")]
    [SerializeField] private Color playerColor = new(0.35f, 0.8f, 1f, 1f);
    [SerializeField] private Color enemyColor = new(1f, 0.4f, 0.4f, 1f);

    private void OnEnable()
    {
        if (turnManager != null)
            turnManager.OnTurnStarted += Refresh;
    }

    private void Start()
    {
        if (turnManager == null)
        {
            Debug.LogError("BattleTurnIndicatorUI: asigna TurnManager en el Inspector.", this);
            return;
        }

        if (turnNumberText == null || turnTeamText == null)
        {
            Debug.LogError("BattleTurnIndicatorUI: asigna los textos del turno.", this);
            return;
        }

        Refresh(turnManager.CurrentCharacter);
    }

    private void OnDisable()
    {
        if (turnManager != null)
            turnManager.OnTurnStarted -= Refresh;
    }

    private void Refresh(BattleCharacter character)
    {
        if (character == null || turnNumberText == null || turnTeamText == null)
            return;

        turnNumberText.text = $"TURNO {turnManager.CurrentTurnId:00}";

        bool isPlayerTurn = character.Team == Team.Player;
        turnTeamText.text = isPlayerTurn ? "JUGADOR" : "ENEMIGO";
        turnTeamText.color = isPlayerTurn ? playerColor : enemyColor;
    }
}
