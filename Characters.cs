namespace TextAdventure;

// INHERITANCE: "abstract" means you can't create a Character directly.
// It only exists to be inherited by more specific classes.
// (C# uses ":" where Java uses the "extends" keyword.)
public abstract class Character
{
    public string Name { get; }
    public Stats Stats;

    protected Character(string name, Stats stats)
    {
        Name = name;
        Stats = stats;
    }

    // Every kind of character must write its own attack message.
    public abstract string AttackMessage();

    // "virtual" means child classes MAY change this, but don't have to.
    public virtual int RollDamage(Random rng)
    {
        return rng.Next(Stats.Attack - 2, Stats.Attack + 3);
    }

    public void TakeDamage(int amount)
    {
        Stats.Health = Math.Max(0, Stats.Health - amount);
    }

    public string HealthBar()
    {
        int filled = (int)Math.Round(10.0 * Stats.Health / Stats.MaxHealth);
        return $"[{new string('#', filled)}{new string('-', 10 - filled)}] {Stats.Health}/{Stats.MaxHealth}";
    }
}

public class Player : Character
{
    public int Gold { get; set; }
    public int Potions { get; set; }
    public bool HasLantern { get; set; }

    public Player(string name) : base(name, new Stats(30, 6))
    {
        Gold = 0;
        Potions = 1;
    }

    public override string AttackMessage() => $"{Name} swings a rusty sword!";

    public void Heal(int amount)
    {
        Stats.Health = Math.Min(Stats.MaxHealth, Stats.Health + amount);
    }

    // Uses the Reward union: the Kind decides which field we read.
    public void Collect(Reward reward)
    {
        switch (reward.Kind)
        {
            case RewardKind.Gold:
                Gold += reward.Gold;
                break;
            case RewardKind.Healing:
                Potions++;
                break;
        }
        Console.WriteLine($"You found {reward}!");
    }
}

public class Goblin : Character
{
    public Goblin() : base("Goblin", new Stats(12, 4)) { }

    public override string AttackMessage() => "The goblin jabs at you with a sharp stick!";
}

public class Troll : Character
{
    public Troll() : base("Cave Troll", new Stats(28, 7)) { }

    public override string AttackMessage() => "The troll slams its huge club down!";

    // Override: the troll sometimes misses because it's slow.
    public override int RollDamage(Random rng)
    {
        bool misses = rng.Next(0, 4) == 0; // 25% chance
        return misses ? 0 : base.RollDamage(rng);
    }
}
