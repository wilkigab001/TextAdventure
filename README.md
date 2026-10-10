# Overview

I wanted to learn C# because it's used for everything from games to web apps, and it's similar to the other languages I've worked with. I built a text adventure game to practice the language and get comfortable with classes and inheritance.

The Lost Lantern is a text-based adventure where you explore a cave, choose which tunnels to take, fight a goblin and a cave troll, and collect treasure. You can save your game and continue later, and winning scores are saved to a high score list.

I wrote this to practice C# basics like variables, loops, conditionals, and functions. I also used classes, structs, a union-style struct, abstract classes with inheritance, and reading and writing files.

To run it, open a terminal in the project folder and type `dotnet run`.

[Software Demo Video](https://www.loom.com/share/1382dc80bbc34945b283209001997844)

# Development Environment

I used Visual Studio Code and the .NET SDK, and ran the game from the terminal with `dotnet run`.

The program is written in C# on .NET 8 and only uses the built-in .NET libraries (`System.IO` for files and `System.Runtime.InteropServices` for the union struct).

# Useful Websites

- [C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Wikipedia: C Sharp (programming language)](https://en.wikipedia.org/wiki/C_Sharp_(programming_language))
- [W3Schools C# Tutorial](https://www.w3schools.com/cs/)

# Future Work

- Add more rooms and enemies
- Add an inventory system for weapons and items
- Save the game as JSON instead of plain text
