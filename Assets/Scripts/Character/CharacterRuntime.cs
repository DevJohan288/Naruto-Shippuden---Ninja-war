using System;

[Serializable]
public class CharacterRuntime
{
    public CharacterData data;

    public int currentHP;
    public int currentChakra;

    public bool IsAlive => currentHP > 0;

    public CharacterRuntime(CharacterData data)
    {
        this.data = data;

        currentHP = data.maxHP;
        currentChakra = Math.Max(
            0,
            Math.Min(data.startingChakra, data.maxChakra)
        );
    }

    public void ReceiveDamage(int damage)
    {
        damage = Math.Max(0, damage);

        currentHP -= damage;

        if (currentHP < 0)
            currentHP = 0;
    }

    public void RestoreChakra(int amount)
    {
        amount = Math.Max(0, amount);
        currentChakra = Math.Min(data.maxChakra, currentChakra + amount);
    }

    public bool TrySpendChakra(int amount)
    {
        amount = Math.Max(0, amount);

        if (currentChakra < amount)
            return false;

        currentChakra -= amount;
        return true;
    }
}
