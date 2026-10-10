// Program.cs
// The Lost Lantern - a text adventure game in C#.
//
// Concepts shown in this project:
//   Variables, expressions, conditionals, loops, functions  (all files)
//   Classes                         -> Game.cs, Characters.cs
//   Structures and unions           -> Structs.cs
//   Inheritance with abstract       -> Characters.cs
//   Reading and writing files       -> SaveManager.cs, ScoreBoard.cs

using TextAdventure;

const string Title = "THE LOST LANTERN";   // a constant can never change
bool running = true;                         // a normal variable can

while (running)
{
    Console.WriteLine();
    Console.WriteLine($"======== {Title} ========");
    Console.WriteLine("1) New game");
    if (SaveManager.SaveExists())
    {
        Console.WriteLine("2) Continue saved game");
    }
    Console.WriteLine("3) High scores");
    Console.WriteLine("4) Quit");

    string choice = Game.Prompt("> ");

    switch (choice)
    {
        case "1":
            string name = Game.Prompt("What's your name, adventurer? ");
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Adventurer";
            }
            new Game(new Player(name), "entrance").Run();
            break;

        case "2" when SaveManager.SaveExists():
            var loaded = SaveManager.Load();
            if (loaded is not null)
            {
                var (player, scene) = loaded.Value;
                Console.WriteLine($"Welcome back, {player.Name}!");
                new Game(player, scene).Run();
            }
            break;

        case "3":
            ScoreBoard.Show();
            break;

        case "4":
            running = false;
            break;

        default:
            Console.WriteLine("Please choose one of the menu options.");
            break;
    }
}

Console.WriteLine("Goodbye!");
