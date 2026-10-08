using UnityEngine;

public class TargetSelectable : MonoBehaviour
{
    private BattleCharacter battleCharacter;

    [SerializeField]
    private GameObject targetMarker;

    private void Awake()
    {
        battleCharacter = GetComponent<BattleCharacter>();

        if (targetMarker != null)
            targetMarker.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (targetMarker != null)
            targetMarker.SetActive(selected);
    }

    private void OnMouseDown()
    {
        if (battleCharacter == null)
            return;

        if (!battleCharacter.IsAlive)
            return;

        if (BattleManager.Instance == null)
            return;

        TargetSystem targetSystem =
            BattleManager.Instance.TargetSystem;

        if (targetSystem == null)
            return;

        targetSystem.SelectTarget(battleCharacter);
    }
}