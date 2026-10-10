namespace TextAdventure;

// FILE I/O: saves the game to a text file and loads it back.
// File format is one "key=value" per line, for example:
//   name=Gabe
//   health=24
public static class SaveManager
{
    public const string SaveFile = "savegame.txt";

    public static void Save(Player player, string scene)
    {
        string[] lines =
        {
            $"name={player.Name}",
            $"health={player.Stats.Health}",
            $"gold={player.Gold}",
            $"potions={player.Potions}",
            $"lantern={player.HasLantern}",
            $"scene={scene}"
        };
        File.WriteAllLines(SaveFile, lines);
        Console.WriteLine($"Game saved to {SaveFile}.");
    }

    public static bool SaveExists() => File.Exists(SaveFile);

    // Returns the player and the scene they were in, or null if loading fails.
    public static (Player player, string scene)? Load()
    {
        try
        {
            var data = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(SaveFile))
            {
                string[] parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    data[parts[0]] = parts[1];
                }
            }

            var player = new Player(data["name"]);
            player.Stats.Health = int.Parse(data["health"]);
            player.Gold = int.Parse(data["gold"]);
            player.Potions = int.Parse(data["potions"]);
            player.HasLantern = bool.Parse(data["lantern"]);
            return (player, data["scene"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Couldn't load the save file: {ex.Message}");
            return null;
        }
    }

    public static void Delete()
    {
        if (File.Exists(SaveFile))
        {
            File.Delete(SaveFile);
        }
    }
}
