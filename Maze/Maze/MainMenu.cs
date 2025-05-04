#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Maze
{
    internal static class MainMenu
    {

        private static MazeGenerator huntKillMaze;
        private static string backText = "Go back to main menu?";
        private static string exitText = "Thank you for using my Maze Generator, Goodbye!";
        private static string invalidText = "Invalid input, please try again";
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

        public static void Setup(MazeGenerator huntAndKill, MazePlayer plr)
        {
            huntKillMaze = huntAndKill;
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
                    Console.WriteLine("Not yet implemented");
                    Console.ResetColor();
                    GoBackToMenu();
                    break;
                case '4':
                    Console.WriteLine(exitText);
                    return;
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