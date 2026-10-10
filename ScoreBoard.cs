namespace TextAdventure;

// FILE I/O: appends each winning score to a text file and reads them back
// to show a top-5 leaderboard. Format per line: name,score
public static class ScoreBoard
{
    private const string ScoreFile = "scores.txt";

    public static void Record(string name, int score)
    {
        File.AppendAllText(ScoreFile, $"{name},{score}{Environment.NewLine}");
    }

    public static void Show()
    {
        if (!File.Exists(ScoreFile))
        {
            Console.WriteLine("No high scores yet. Go win a game!");
            return;
        }

        var scores = new List<(string name, int score)>();
        foreach (string line in File.ReadAllLines(ScoreFile))
        {
            string[] parts = line.Split(',');
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
            {
                scores.Add((parts[0], value));
            }
        }

        // Sort highest first and keep the top 5.
        var top = scores.OrderByDescending(s => s.score).Take(5).ToList();

        Console.WriteLine("\n===== HIGH SCORES =====");
        for (int i = 0; i < top.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {top[i].name,-12} {top[i].score,5}");
        }
    }
}
