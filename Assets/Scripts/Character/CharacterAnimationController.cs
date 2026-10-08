
using System;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private readonly int isMovingHash =
        Animator.StringToHash("IsMoving");

    private readonly int attackHash =
        Animator.StringToHash("Attack");

    private readonly int deathHash =
        Animator.StringToHash("Death");

    private bool originalFlipX;
    private bool hasDeathTrigger;


    // =========================================================
    // EVENTOS
    // =========================================================

    public event Action OnAttackHit;
    public event Action OnAttackFinished;
    public event Action OnDeathFinished;


    // =========================================================
    // INICIALIZACIÓN
    // =========================================================

    private void Awake()
    {
        animator =
            GetComponent<Animator>();

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        hasDeathTrigger = HasTrigger("Death");


        if (spriteRenderer != null)
        {
            originalFlipX =
                spriteRenderer.flipX;
        }
    }


    // =========================================================
    // MOVIMIENTO
    // =========================================================

    public void SetMoving(bool moving)
    {
        if (animator == null)
            return;


        animator.SetBool(
            isMovingHash,
            moving
        );
    }


    // =========================================================
    // IDLE
    // =========================================================

    public void PlayIdle()
    {
        if (animator == null)
            return;


        animator.SetBool(
            isMovingHash,
            false
        );


        animator.ResetTrigger(
            attackHash
        );


        if (hasDeathTrigger)
            animator.ResetTrigger(deathHash);
    }


    // =========================================================
    // ATAQUE
    // =========================================================

    public void PlayAttack()
    {
        if (animator == null)
            return;


        animator.SetBool(
            isMovingHash,
            false
        );


        if (hasDeathTrigger)
            animator.ResetTrigger(deathHash);


        animator.SetTrigger(
            attackHash
        );
    }


    // =========================================================
    // MUERTE
    // =========================================================

    public void PlayDeath()
    {
        if (animator == null)
            return;


        animator.SetBool(
            isMovingHash,
            false
        );


        animator.ResetTrigger(
            attackHash
        );


        if (hasDeathTrigger)
            animator.SetTrigger(deathHash);
        else
            OnDeathFinished?.Invoke();
    }


    // =========================================================
    // EVENTO - GOLPE
    // =========================================================

    public void AnimationEventAttackHit()
    {
        OnAttackHit?.Invoke();
    }


    // =========================================================
    // EVENTO - ATAQUE TERMINADO
    // =========================================================

    public void AnimationEventAttackFinished()
    {
        OnAttackFinished?.Invoke();
    }


    // =========================================================
    // EVENTO - MUERTE TERMINADA
    // =========================================================

    public void AnimationEventDeathFinished()
    {
        OnDeathFinished?.Invoke();
    }


    // =========================================================
    // DIRECCIÓN
    // =========================================================

    public void FaceDirection(
        Vector3 direction)
    {
        if (spriteRenderer == null)
            return;


        if (Mathf.Abs(direction.x) < 0.01f)
            return;


        spriteRenderer.flipX =
            direction.x < 0f;
    }


    // =========================================================
    // ORIENTACIÓN ORIGINAL
    // =========================================================

    public void SaveCurrentDirection()
    {
        if (spriteRenderer == null)
            return;


        originalFlipX =
            spriteRenderer.flipX;
    }


    public void RestoreOriginalDirection()
    {
        if (spriteRenderer == null)
            return;


        spriteRenderer.flipX =
            originalFlipX;
    }


    // =========================================================
    // OBTENER ORIENTACIÓN
    // =========================================================

    public bool GetCurrentFlipX()
    {
        if (spriteRenderer == null)
            return false;


        return spriteRenderer.flipX;
    }


    // =========================================================
    // ESTABLECER ORIENTACIÓN
    // =========================================================

    public void SetFlipX(
        bool flipX)
    {
        if (spriteRenderer == null)
            return;


        spriteRenderer.flipX =
            flipX;
    }

    private bool HasTrigger(string parameterName)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName &&
                parameter.type == AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }
}
