using UnityEngine;

[CreateAssetMenu(
    fileName = "Character",
    menuName = "Naruto RPG/Character Data"
)]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterID;
    public string characterName;

    [Header("Base Stats")]
    public int maxHP = 100;
    public int attack = 10;
    public int defense = 5;
    public int speed = 10;

    [Header("Chakra")]
    public int maxChakra = 100;

    [Min(0)]
    public int startingChakra = 0;

    [Min(0)]
    public int chakraGainOnBasicAttack = 10;

    [Header("Visual")]
    public Sprite portrait;
}
