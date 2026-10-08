using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterAnimationController))]
public class BattleCharacter : MonoBehaviour
{
    public event System.Action<BattleCharacter> OnHealthChanged;
    public event System.Action<BattleCharacter> OnChakraChanged;
    public event System.Action<BattleCharacter> OnDied;

    [Header("Character Data")]
    [SerializeField]
    private CharacterData characterData;

    [SerializeField]
private float deathVisibleDelay = 3f;

[SerializeField]
private float deathFadeDuration = 0.35f;

    public CharacterData Data =>
        characterData;

    public CharacterRuntime Runtime
    {
        get;
        private set;
    }

    public CharacterAnimationController AnimationController
    {
        get;
        private set;
    }

    public Team Team
    {
        get;
        private set;
    }

    public bool IsAlive =>
        Runtime != null &&
        Runtime.IsAlive;

    public int Speed =>
        Data != null
            ? Data.speed
            : 0;


    private Vector3 battlePosition;
    private bool battleFlipX;

    private Collider2D characterCollider;
    private SpriteRenderer spriteRenderer;

    private bool deathHandled;


    // =========================================================
    // INICIALIZACIÓN
    // =========================================================

    public void Initialize(Team team)
    {
        Team = team;

        deathHandled = false;


        if (characterData == null)
        {
            Debug.LogError(
                $"{name}: No tiene CharacterData asignado.",
                this
            );

            return;
        }


        Runtime =
            new CharacterRuntime(
                characterData
            );


        AnimationController =
            GetComponent<CharacterAnimationController>();


        if (AnimationController == null)
        {
            Debug.LogError(
                $"{name}: Falta CharacterAnimationController.",
                this
            );

            return;
        }


        characterCollider =
            GetComponent<Collider2D>();


        spriteRenderer =
            GetComponent<SpriteRenderer>();


        if (characterCollider != null)
        {
            characterCollider.enabled = true;
        }


        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }


        battlePosition =
            transform.position;


        battleFlipX =
            AnimationController.GetCurrentFlipX();


        AnimationController.PlayIdle();


        AnimationController.OnDeathFinished +=
            HandleDeathFinished;

        OnHealthChanged?.Invoke(this);
        OnChakraChanged?.Invoke(this);
    }


    // =========================================================
    // DAÑO
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
            return;


        if (damage < 0)
        {
            damage = 0;
        }


        Runtime.ReceiveDamage(
            damage
        );

        OnHealthChanged?.Invoke(this);


        if (!IsAlive)
        {
            Die();
        }
    }

    public void GainChakra(int amount)
    {
        if (Runtime == null || amount <= 0)
            return;

        int previousChakra = Runtime.currentChakra;
        Runtime.RestoreChakra(amount);

        if (Runtime.currentChakra != previousChakra)
            OnChakraChanged?.Invoke(this);
    }

    public bool TrySpendChakra(int amount)
    {
        if (Runtime == null || !Runtime.TrySpendChakra(amount))
            return false;

        OnChakraChanged?.Invoke(this);
        return true;
    }


    // =========================================================
    // MUERTE
    // =========================================================

    private void Die()
    {
        if (deathHandled)
            return;


        deathHandled = true;


        // -----------------------------------------------------
        // Ya no puede ser objetivo/interactuar.
        // -----------------------------------------------------

        if (characterCollider != null)
        {
            characterCollider.enabled = false;
        }

        OnDied?.Invoke(this);


        // -----------------------------------------------------
        // Ejecutar animación de muerte.
        // -----------------------------------------------------

        if (AnimationController != null)
        {
            AnimationController.PlayDeath();
        }
        else
        {
            HideCharacterVisual();
        }
    }


    // =========================================================
    // FIN DE MUERTE
    // =========================================================

    private void HandleDeathFinished()
     {
    StartCoroutine(
        FadeOutAfterDeath()
    );
    }

    private IEnumerator FadeOutAfterDeath()
{
    if (spriteRenderer == null)
        yield break;


    // -----------------------------------------------------
    // Mantener personaje visible durante 1 segundo
    // -----------------------------------------------------

    yield return new WaitForSeconds(
        deathVisibleDelay
    );


    // -----------------------------------------------------
    // Fade Out
    // -----------------------------------------------------

    Color color =
        spriteRenderer.color;

    float elapsed = 0f;


    while (elapsed < deathFadeDuration)
    {
        elapsed += Time.deltaTime;

        float alpha =
            Mathf.Lerp(
                1f,
                0f,
                elapsed / deathFadeDuration
            );

        color.a = alpha;

        spriteRenderer.color =
            color;

        yield return null;
    }


    // -----------------------------------------------------
    // Asegurar transparencia total
    // -----------------------------------------------------

    color.a = 0f;

    spriteRenderer.color =
        color;


    spriteRenderer.enabled = false;
}





    // =========================================================
    // OCULTAR PERSONAJE
    // =========================================================

    private void HideCharacterVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }


    // =========================================================
    // POSICIÓN
    // =========================================================

    public void ReturnToBattlePosition()
    {
        transform.position =
            battlePosition;
    }


    public Vector3 GetBattlePosition()
    {
        return battlePosition;
    }


    // =========================================================
    // ORIENTACIÓN
    // =========================================================

    public void RestoreBattleOrientation()
    {
        if (AnimationController == null)
            return;

        AnimationController.SetFlipX(
            battleFlipX
        );
    }


    // =========================================================
    // LIMPIEZA
    // =========================================================

    private void OnDestroy()
    {
        if (AnimationController != null)
        {
            AnimationController.OnDeathFinished -=
                HandleDeathFinished;
        }
    }
}
