#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maze
{
    internal static class MainMenu
    {

        private static MazeGenerator huntKillMaze;
        private static string backText = "Go back to main menu?";
        private static string exitText = "Thank you for using my Maze Generator, Goodbye!";
        private static string invalidText = "Invalid input, please try again";
        private static ConfigData config;
        private static MazePlayer player;
        private static string[] menuParts = new string[]
        {
            "╔════════════════════════╗",
            "║       MAIN MENU        ║",
            "╠════════════════════════╣",
            "║ 1. Generate Maze       ║",
            "║ 2. Walk through maze   ║",
            "║ 3. Settings            ║",
            "║ 4. Exit                ║",
            "╚════════════════════════╝"
        };

        public static void Setup(MazeGenerator huntAndKill, MazePlayer plr, ConfigData newConfig)
        {
            huntKillMaze = huntAndKill;
            config = newConfig;
            player = plr;
        }

        public static void ShowMenu()
        {
            Console.Clear();
            Console.Title = "Main Menu - Maze Generator";

            foreach (string str in menuParts)
            {
                Console.WriteLine(str);
            }

            Console.Write("Select an option: ");

            char[] inputArray = {' '};
            bool validInput = false;
            
            Console.CursorVisible = true;

            while (validInput == false)
            {
                inputArray = Console.ReadLine().Trim().ToCharArray();
                
                if (inputArray.Length > 0)
                {
                    if (char.IsNumber(inputArray[0]) && char.GetNumericValue(inputArray[0]) <= 4)
                    {
                        validInput = true;
                        continue;
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(invalidText);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(invalidText);
                    Console.ResetColor();
                }
               
            }

            switch (inputArray[0])
            {
                case '1':
                    Console.Clear();
                    huntKillMaze.GenerateMaze();
                    break;
                case '2':
                    Console.Clear();
                    if (player.HasGenerated())
                    {
                        player.StartPlayerMovement();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Can't enter play mode before generating a maze!");
                        Console.ResetColor();
                        GoBackToMenu();
                    }
                    break;
                case '3':
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Clear();
                    ShowSettings();
                    ChangeSettings();
                    Console.ResetColor();
                    GoBackToMenu();
                    break;
                case '4':
                    Console.WriteLine(exitText);
                    return;
            }
        }

        public static void ShowSettings()
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("----- Available settings -----");
            Dictionary<string, object> variables = config.GetVariables();
            foreach (var item in variables)
            {
                Console.Write("- " + item.Key + ": ");
                if (item.Value is Array array)
                {
                    Type? type = array.GetType().GetElementType();
                    if (array.Rank == 1)
                    {
                        int length = array.Length;
                        Console.Write($"new {type.Name}[{length}]" + " { ");
                        for (int i = 0; i < length; i++)
                        {
                            if (i == length - 1)
                            {
                                Console.Write(array.GetValue(i).ToString() + " }\n");
                            }
                            else
                            {
                                Console.Write(array.GetValue(i).ToString() + ", ");
                            }
                        }
                    }
                    else if (array.Rank == 2)
                    {
                        int xLength = array.GetLength(0);
                        int yLength = array.GetLength(1);
                        Console.Write($"new {type.Name}[{yLength}, {xLength}]\n");
                    }
                }
                else
                {
                    Console.Write(item.Value.ToString() + "\n");
                }
                
            }
            Console.WriteLine("------------------------------");
            Console.WriteLine("Type a variable and then its corresponding value to change it.");
            Console.WriteLine("To go back, type 'back'.");
            Console.WriteLine("------------------------------");
        }

        private static void ChangeSettings()
        {
            Dictionary<string, object> variables = config.GetVariables();
            bool active = true;
            while (active)
            {
                Console.Write("\n\rInput: ");
                string? input = Console.ReadLine();
                if (input == null) { Console.WriteLine("Invalid input, try again."); continue; }
                if (input.ToLower() == "back")
                {
                    active = false;
                    ShowMenu();
                }

                string[] parts = input.Split(' ');

                if (parts.Length < 2)
                {
                    Console.WriteLine("Invalid input, try again.");
                    continue;
                }

                string key = parts[0];
                string value = parts[1];

                if (!variables.ContainsKey(parts[0]))
                {
                    Console.WriteLine($"Variable {key} not found, try again.");
                    continue;
                }

                if (!variables.TryGetValue(key, out object? configValue) || configValue == null)
                {
                    Console.WriteLine($"Value {value} for key {key} not found, try again.");
                    continue;
                }

                if (configValue.GetType().IsArray)
                {
                    Console.WriteLine("Array support not added yet.");
                    continue;
                }
                TypeConverter converter = TypeDescriptor.GetConverter(configValue.GetType());
                if (!converter.IsValid(value))
                {
                    Console.WriteLine("Invalid value, try again.");
                }
                object? convertedValue = converter.ConvertFromString(value);
                if (convertedValue == null || convertedValue.GetType() != configValue.GetType())
                {
                    Console.WriteLine("Value is null or not matching.");
                    continue;
                }

                config.SetValue(parts[0], convertedValue);
                ShowSettings();
                Console.WriteLine($"Sucessfully set {key} to {convertedValue.ToString()}.");
            }
        }

        public static void GoBackToMenu()
        {
            Console.Write(backText);
            Console.Write("(");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("y");
            Console.ResetColor();
            Console.Write("/");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("n");
            Console.ResetColor();
            Console.Write(")");
            Console.WriteLine();
            Console.CursorVisible = true;

            bool valid = false;
            while (!valid)
            {
                char[] inputArray = Console.ReadLine().Trim().ToLower().ToCharArray();
                
                if (inputArray.Length > 0)
                {
                    char input = inputArray[0];
                    if (input == 'y')
                    {
                        valid = true;
                        ShowMenu();
                    }
                    else if (input == 'n')
                    {
                        valid = true;
                        Console.WriteLine(exitText);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(invalidText);
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(invalidText);
                    Console.ResetColor();
                }
            }
        }
    }
}