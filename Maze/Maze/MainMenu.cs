#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maze
{
    internal static class MainMenu
    {
        private static string backText = "Go back to main menu?";
        private static string exitText = "Thank you for using my Maze Generator, Goodbye!";
        private static string invalidText = "Invalid input, please try again";

        private static MazePlayer player;
        private static MazeGenerator huntKillMaze;
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

        public static void Setup(MazeGenerator huntAndKill, MazePlayer plr)
        {
            huntKillMaze = huntAndKill;
            player = plr;
        }

        /// <summary>
        /// Displays the main menu
        /// </summary>
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
                    Environment.Exit(0);
                    return;
            }
        }

        /// <summary>
        /// Displays the config settings
        /// </summary>
        public static void ShowSettings()
        {
            Console.Title = "Settings - Maze Generator";
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("----- Available settings -----");

            Dictionary<string, ConfigEntry> variables = ConfigData.GetVariables(); 
            foreach (var item in variables)
            {
                Console.Write("- " + item.Key + ": ");

                // If item is of type Array
                if (item.Value.value is Array array)
                {
                    Type? type = array.GetType().GetElementType(); // Gets the type of the elements inside the array
                    if (array.Rank == 1) // If it's a 1D array
                    {
                        int length = array.Length;
                        Console.Write($"new {type.Name}[{length}]" + " { ");

                        for (int i = 0; i < length; i++) // Outputs all the elements in the array
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
                    else if (array.Rank == 2) // If it's a 2D array
                    {
                        int xLength = array.GetLength(0);
                        int yLength = array.GetLength(1);
                        Console.Write($"new {type.Name}[{yLength}, {xLength}]\n");
                    }
                    else
                    {
                        Console.WriteLine($"Array rank: {array.Rank} not supported yet");
                    }
                }
                else
                {
                    Console.Write(item.Value.value.ToString() + "\n");
                }
                
            }
            Console.WriteLine("------------------------------");
            Console.WriteLine("Type a variable and then its corresponding new value to change it.");
            Console.WriteLine("To go back, type 'back'.");
            Console.WriteLine("------------------------------");
        }

        /// <summary>
        /// Allows the user to edit values in the config by specifying a key and a value
        /// </summary>
        private static void ChangeSettings()
        {
            Dictionary<string, ConfigEntry> variables = ConfigData.GetVariables();
            bool active = true;

            while (active)
            {
                Console.Write("\n\rInput: ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))  
                {
                    ShowSettings();
                    Console.WriteLine("Invalid input, try again."); 
                    continue; 
                }

                if (input.ToLower() == "back")
                {
                    active = false;
                    ShowMenu();
                    return;
                }

                string[] splitInput = input.Split(' '); // Splits the string by whitespaces to a string array

                if (splitInput.Length < 2)
                {
                    ShowSettings();
                    Console.WriteLine("Invalid input, try again.");
                    continue;
                }

                string key = splitInput[0];
                string value = splitInput[1];

                if (!variables.ContainsKey(splitInput[0]))
                {
                    ShowSettings();
                    Console.WriteLine($"Variable {key} not found, try again.");
                    continue;
                }

                // Tries to get the value of the specified key, continues the loop if it doesnt find any value
                if (!variables.TryGetValue(key, out ConfigEntry? configValue) || configValue == null)
                {
                    ShowSettings();
                    Console.WriteLine($"Value {value} for key {key} not found, try again.");
                    continue;
                }

                var valueType = configValue.value.GetType();

                if (valueType.IsArray)
                {
                    ShowSettings();
                    Console.WriteLine($"Arrays are not supported yet, will get added in a future update.");
                    continue;
                }

                // Gets the TypeConverter for the type of the key's value
                TypeConverter converter = TypeDescriptor.GetConverter(configValue.value.GetType());

                try
                {
                    var convertedValue = converter.ConvertFromString(value); // Tries to convert the string value to the key's value
                    if (convertedValue == null || convertedValue.GetType() != configValue.value.GetType())
                    {
                        ShowSettings();
                        Console.WriteLine("Value is null or not matching.");
                        continue;
                    }
                    ConfigData.SetValue(key, convertedValue); // Sets the key's value to the new value
                    ShowSettings();
                    Console.WriteLine($"Sucessfully set {key} to {convertedValue.ToString()}.");
                }
                catch (Exception ex)
                {
                    ShowSettings();
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Prompts the user to go back to the main menu
        /// </summary>
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
                // Removes all whitespaces, makes all characters to lower characters and converts the input to a char array
                char[] inputArray = Console.ReadLine().Trim().ToLower().ToCharArray();
                
                if (inputArray.Length > 0)
                {
                    char input = inputArray[0];
                    if (input == 'y')
                    {
                        valid = true;
                        ShowMenu();
                    }
                    else if (input == 'n') // Exits the program if input is 'n'
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