using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private List<BattleCharacter> turnOrder = new();

    private int currentIndex;

    private bool isEndingTurn;

    public System.Action<BattleCharacter> OnTurnStarted;

    public BattleCharacter CurrentCharacter { get; private set; }

    public int CurrentTurnId { get; private set; }


    // =========================================================
    // INICIALIZACIÓN
    // =========================================================

    public void Initialize(
        List<BattleCharacter> players,
        List<BattleCharacter> enemies)
    {
        turnOrder.Clear();

        turnOrder.AddRange(players);
        turnOrder.AddRange(enemies);

        turnOrder = turnOrder
            .Where(character =>
                character != null)
            .OrderByDescending(character =>
                character.Speed)
            .ToList();

        currentIndex = 0;

        CurrentTurnId = 0;

        isEndingTurn = false;

        StartTurn();
    }


    // =========================================================
    // INICIAR TURNO
    // =========================================================

    private void StartTurn()
    {
        if (turnOrder.Count == 0)
            return;


        isEndingTurn = false;


        int checkedCharacters = 0;


        while (checkedCharacters < turnOrder.Count)
        {
            BattleCharacter character =
                turnOrder[currentIndex];


            if (character != null &&
                character.IsAlive)
            {
                CurrentCharacter =
                    character;


                CurrentTurnId++;


                OnTurnStarted?.Invoke(
                    CurrentCharacter
                );


                return;
            }


            currentIndex++;

            if (currentIndex >= turnOrder.Count)
            {
                currentIndex = 0;
            }


            checkedCharacters++;
        }


        CurrentCharacter = null;


        Debug.Log(
            "No quedan personajes vivos para continuar."
        );
    }


    // =========================================================
    // TERMINAR TURNO
    // =========================================================

    public void EndTurn()
    {
        // -----------------------------------------------------
        // Evitar terminar el mismo turno dos veces.
        // -----------------------------------------------------

        if (isEndingTurn)
        {
            Debug.LogWarning(
                "TurnManager: EndTurn() ignorado porque " +
                "el turno ya está terminando."
            );

            return;
        }


        if (turnOrder.Count == 0)
            return;


        isEndingTurn = true;


        currentIndex++;


        if (currentIndex >= turnOrder.Count)
        {
            currentIndex = 0;
        }


        StartTurn();
    }
}
