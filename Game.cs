namespace TextAdventure;

// CLASS: Game holds the state of one play-through and runs the scenes.
public class Game
{
    private readonly Player _player;
    private readonly Random _rng = new Random();
    private string _scene;

    public Game(Player player, string startScene)
    {
        _player = player;
        _scene = startScene;
    }

    // Main game LOOP: keep running scenes until we reach "end" or "quit".
    public void Run()
    {
        while (_scene != "end" && _scene != "quit")
        {
            Console.WriteLine();
            _scene = _scene switch
            {
                "entrance" => Entrance(),
                "left" => LeftTunnel(),
                "right" => RightTunnel(),
                "bridge" => Bridge(),
                "troll" => TrollLair(),
                _ => "end"
            };

            if (_player.Stats.IsDefeated)
            {
                GameOver();
                return;
            }
        }

        if (_scene == "quit")
        {
            Console.WriteLine("Thanks for playing! Your progress was saved.");
        }
    }

    // ---------------- Scenes ----------------
    // Each scene is a FUNCTION that returns the name of the next scene.

    private string Entrance()
    {
        Say("You stand at the mouth of a dark cave. Somewhere inside is the");
        Say("Lost Lantern, and a cave troll who doesn't want to share it.");
        Say("The path splits into two tunnels.");
        return Choose("entrance", new[]
        {
            ("Take the LEFT tunnel (you hear scratching)", "left"),
            ("Take the RIGHT tunnel (you see a faint glimmer)", "right")
        });
    }

    private string LeftTunnel()
    {
        Say("A goblin jumps out from behind a rock!");
        bool won = Fight(new Goblin());
        if (!won)
        {
            return _player.Stats.IsDefeated ? "end" : "entrance";
        }

        Say("Behind the goblin you find an old lantern. You light it.");
        _player.HasLantern = true;
        _player.Collect(Reward.MakeGold(15));
        return Choose("left", new[] { ("Continue deeper into the cave", "bridge") });
    }

    private string RightTunnel()
    {
        Say("You find a small wooden chest covered in dust.");
        string next = Choose("right", new[]
        {
            ("Open the chest", "open"),
            ("Leave it alone and keep going", "bridge")
        });

        if (next == "open")
        {
            // CONDITIONAL with a random roll: 50/50 gold or a potion.
            Reward reward = _rng.Next(2) == 0
                ? Reward.MakeGold(25)
                : Reward.MakeHealing(10);
            _player.Collect(reward);
            next = "bridge";
        }
        return next;
    }

    private string Bridge()
    {
        Say("You reach a narrow rope bridge over a deep chasm.");

        if (_player.HasLantern)
        {
            Say("Your lantern lights the way and you cross safely.");
        }
        else
        {
            Say("It's too dark to see the boards. You stumble across...");
            // EXPRESSION: damage scales with how hurt you already are.
            int fallDamage = 3 + (_player.Stats.MaxHealth - _player.Stats.Health) / 5;
            _player.TakeDamage(fallDamage);
            Say($"You twist your ankle and lose {fallDamage} HP. {_player.HealthBar()}");
        }

        return Choose("bridge", new[] { ("Enter the troll's lair", "troll") });
    }

    private string TrollLair()
    {
        Say("A huge cave troll guards a pile of treasure. It roars!");
        bool won = Fight(new Troll(), canRun: false);
        if (!won)
        {
            return "end";
        }

        Say("The troll crashes to the ground. You claim its treasure!");
        _player.Collect(Reward.MakeGold(50));
        Victory();
        return "end";
    }

    // ---------------- Combat ----------------

    // Returns true if the player wins, false if they lose or run away.
    // Takes a Character, so it works for ANY enemy type (polymorphism).
    private bool Fight(Character enemy, bool canRun = true)
    {
        while (!enemy.Stats.IsDefeated && !_player.Stats.IsDefeated)
        {
            Console.WriteLine();
            Say($"{_player.Name,-10} {_player.HealthBar()}");
            Say($"{enemy.Name,-10} {enemy.HealthBar()}");
            Console.WriteLine("  1) Attack");
            Console.WriteLine($"  2) Drink potion ({_player.Potions} left)");
            if (canRun) Console.WriteLine("  3) Run away");

            string input = Prompt("> ");

            if (input == "1")
            {
                int damage = _player.RollDamage(_rng);
                enemy.TakeDamage(damage);
                Say($"{_player.AttackMessage()} You deal {damage} damage.");
            }
            else if (input == "2")
            {
                if (_player.Potions > 0)
                {
                    _player.Potions--;
                    _player.Heal(10);
                    Say("You drink a potion and feel better.");
                }
                else
                {
                    Say("You're out of potions!");
                    continue; // don't let the enemy attack for a wasted turn
                }
            }
            else if (input == "3" && canRun)
            {
                Say("You run back the way you came!");
                return false;
            }
            else
            {
                Say("That's not an option.");
                continue;
            }

            // Enemy's turn, only if it's still standing.
            if (!enemy.Stats.IsDefeated)
            {
                int enemyDamage = enemy.RollDamage(_rng);
                _player.TakeDamage(enemyDamage);
                Say(enemyDamage > 0
                    ? $"{enemy.AttackMessage()} You take {enemyDamage} damage."
                    : $"The {enemy.Name} misses!");
            }
        }

        if (enemy.Stats.IsDefeated)
        {
            Say($"You defeated the {enemy.Name}!");
            return true;
        }
        return false;
    }

    // ---------------- Endings ----------------

    private void Victory()
    {
        int score = _player.Gold + _player.Stats.Health * 2;
        Console.WriteLine();
        Say("*** YOU WIN! ***");
        Say($"Gold: {_player.Gold}   Health left: {_player.Stats.Health}   Score: {score}");
        SaveManager.Delete();
        ScoreBoard.Record(_player.Name, score);
        ScoreBoard.Show();
    }

    private void GameOver()
    {
        Console.WriteLine();
        Say("You collapse on the cold cave floor... GAME OVER.");
        SaveManager.Delete();
    }

    // ---------------- Helpers ----------------

    // Shows numbered choices plus Save / Quit, and loops until valid input.
    private string Choose(string currentScene, (string text, string next)[] options)
    {
        while (true)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"  {i + 1}) {options[i].text}");
            }
            Console.WriteLine("  S) Save game    Q) Save and quit");

            string input = Prompt("> ").ToUpper();

            if (input == "S")
            {
                SaveManager.Save(_player, currentScene);
            }
            else if (input == "Q")
            {
                SaveManager.Save(_player, currentScene);
                return "quit";
            }
            else if (int.TryParse(input, out int pick) && pick >= 1 && pick <= options.Length)
            {
                return options[pick - 1].next;
            }
            else
            {
                Say("Pick one of the options above.");
            }
        }
    }

    public static string Prompt(string message)
    {
        Console.Write(message);
        string? line = Console.ReadLine();
        if (line == null)
        {
            // Input stream closed (e.g. Ctrl+D / Ctrl+Z), so exit cleanly.
            Environment.Exit(0);
        }
        return line.Trim();
    }

    private static void Say(string text) => Console.WriteLine(text);
}
