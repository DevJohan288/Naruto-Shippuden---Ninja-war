using System.Collections;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField]
    private float attackDistance = 1.2f;


    // =========================================================
    // MOVER HACIA EL ATAQUE
    // =========================================================

    public IEnumerator MoveToAttackPosition(
        BattleCharacter attacker,
        BattleCharacter target)
    {
        if (attacker == null || target == null)
            yield break;


        // Guardamos la orientación original
        // antes de comenzar el ataque.
        attacker.AnimationController?.
            SaveCurrentDirection();


        Vector3 targetPosition =
            GetAttackPosition(
                attacker,
                target
            );


        Vector3 direction =
            targetPosition -
            attacker.transform.position;


        // Mirar hacia el enemigo.
        attacker.AnimationController?.
            FaceDirection(direction);


        yield return MoveToPosition(
            attacker,
            targetPosition
        );
    }


    // =========================================================
    // TELEPORTAR A POSICIÓN ORIGINAL
    // =========================================================

    public void TeleportToBattlePosition(
        BattleCharacter attacker)
    {
        if (attacker == null)
            return;


        Vector3 battlePosition =
            attacker.GetBattlePosition();


        // Detener movimiento normal.
        attacker.AnimationController?.
            SetMoving(false);


        // Teleportar directamente.
        attacker.transform.position =
            battlePosition;


        Debug.Log(
            $"{attacker.Data.characterName} fue " +
            "teletransportado a su posición de batalla."
        );
    }


    // =========================================================
    // MOVIMIENTO GENERAL
    // =========================================================

    private IEnumerator MoveToPosition(
        BattleCharacter character,
        Vector3 destination)
    {
        if (character == null)
            yield break;


        Transform transformCharacter =
            character.transform;


        Vector3 movementDirection =
            destination -
            transformCharacter.position;


        // Mirar hacia donde se está moviendo.
        character.AnimationController?.
            FaceDirection(movementDirection);


        // Activar animación de Run.
        character.AnimationController?.
            SetMoving(true);


        while (
            Vector3.Distance(
                transformCharacter.position,
                destination
            ) > 0.01f)
        {
            transformCharacter.position =
                Vector3.MoveTowards(
                    transformCharacter.position,
                    destination,
                    movementSpeed *
                    Time.deltaTime
                );


            yield return null;
        }


        // Asegurar posición exacta.
        transformCharacter.position =
            destination;


        // Detener Run.
        character.AnimationController?.
            SetMoving(false);
    }


    // =========================================================
    // POSICIÓN DE ATAQUE
    // =========================================================

    private Vector3 GetAttackPosition(
        BattleCharacter attacker,
        BattleCharacter target)
    {
        Vector3 direction =
            (
                target.transform.position -
                attacker.transform.position
            ).normalized;


        return target.transform.position -
               direction *
               attackDistance;
    }
}