using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vincula una barra de vida creada en el Editor con el BattleCharacter padre.
/// El prefab no conoce personajes concretos y se actualiza por eventos.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class WorldHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    private BattleCharacter battleCharacter;

    private void Awake()
    {
        battleCharacter = GetComponentInParent<BattleCharacter>();

        if (battleCharacter == null)
            Debug.LogError("WorldHealthBarUI debe ser hijo de un BattleCharacter.", this);

        if (healthFill == null)
            Debug.LogError("WorldHealthBarUI: asigna HealthFill en el Inspector.", this);
    }

    private void OnEnable()
    {
        if (battleCharacter != null)
        {
            battleCharacter.OnHealthChanged += RefreshHealth;
            battleCharacter.OnDied += Hide;
        }
    }

    private void Start()
    {
        RefreshHealth(battleCharacter);
    }

    private void OnDisable()
    {
        if (battleCharacter != null)
        {
            battleCharacter.OnHealthChanged -= RefreshHealth;
            battleCharacter.OnDied -= Hide;
        }
    }

    private void RefreshHealth(BattleCharacter character)
    {
        if (healthFill == null || character == null ||
            character.Runtime == null || character.Data == null)
        {
            return;
        }

        int maximumHealth = Mathf.Max(1, character.Data.maxHP);
        healthFill.fillAmount = Mathf.Clamp01(
            (float)character.Runtime.currentHP / maximumHealth
        );
    }

    private void Hide(BattleCharacter character)
    {
        gameObject.SetActive(false);
    }
}
