using System.Runtime.InteropServices;

namespace TextAdventure;

// STRUCTURE: a struct is a value type. When you copy it, you get a
// completely separate copy (unlike a class, which is copied by reference).
public struct Stats
{
    public int MaxHealth;
    public int Health;
    public int Attack;

    public Stats(int maxHealth, int attack)
    {
        MaxHealth = maxHealth;
        Health = maxHealth;
        Attack = attack;
    }

    // Expression-bodied property: true when health drops to 0 or below.
    public bool IsDefeated => Health <= 0;
}

public enum RewardKind
{
    Gold,
    Healing
}

// UNION: C# has no "union" keyword, but StructLayout.Explicit lets two
// fields share the SAME memory, which is exactly what a C union does.
// Gold and HealAmount both live at byte offset 4, so only one is
// meaningful at a time. Kind (at offset 0) tells us which one to read.
[StructLayout(LayoutKind.Explicit)]
public struct Reward
{
    [FieldOffset(0)] public RewardKind Kind;
    [FieldOffset(4)] public int Gold;
    [FieldOffset(4)] public int HealAmount;

    public static Reward MakeGold(int amount)
    {
        Reward r = new Reward();
        r.Kind = RewardKind.Gold;
        r.Gold = amount;
        return r;
    }

    public static Reward MakeHealing(int amount)
    {
        Reward r = new Reward();
        r.Kind = RewardKind.Healing;
        r.HealAmount = amount;
        return r;
    }

    public override string ToString()
    {
        return Kind == RewardKind.Gold
            ? $"{Gold} gold coins"
            : $"a potion that heals {HealAmount} HP";
    }
}
