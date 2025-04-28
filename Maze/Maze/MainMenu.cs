using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


namespace Maze
{
    internal static class MainMenu
    {

        private static HuntAndKill huntKillMaze;
        private static string backText = "Go back to main menu? (y/n)";
        private static string exitText = "Thank you for using my Maze Generator, Goodbye!";
        private static string invalidText = "Invalid input, please try again";
        private static Player player;
        private static List<ConsoleColor> colorThing = new List<ConsoleColor> { ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green };
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

        public static void SetClass(HuntAndKill huntAndKill, Player plr)
        {
            huntKillMaze = huntAndKill;
            player = plr;
        }

        //private static void doColorThing(bool validInput)
        //{
        //    while (!validInput)
        //    {
        //        Console.SetCursorPosition(0, 0);
        //        int j = 0;
        //        for (int i = 0; i < menuParts.Length; i++)
        //        {
        //            Thread.Sleep(30);
        //            Console.ForegroundColor = colorThing[j];
        //            ConsoleColor[] tempArray = new ConsoleColor[] { colorThing[j] };
        //            colorThing.Remove(colorThing[j]);
        //            colorThing.Add(tempArray[0]);
        //            Console.Write(menuParts[i]);
        //            Console.WriteLine();

        //            if (j < colorThing.Count - 1)
        //            {
        //                j++;
        //            }
        //            else
        //            {
        //                j = 0;
        //            }
                    
        //        }
        //    }
        //}

        public static void ShowMenu()
        {
            Console.Clear();
            Console.Title = "Main Menu - Maze Generator";

            foreach (string str in menuParts)
            {
                Console.WriteLine(str);
            }

            Console.Write("Select an option: ");
            char input = ' ';
            bool validInput = false;
            
            Console.CursorVisible = true;

            while (validInput == false)
            {
                input = Console.ReadLine().ToCharArray()[0];
                if (char.IsNumber(input) && char.GetNumericValue(input) <= 4)
                {
                    validInput = true;
                    continue;
                }
                Console.WriteLine(invalidText);
            }

            switch (input)
            {
                case '1':
                    Console.Clear();
                    huntKillMaze.GenerateMaze();
                    break;
                case '2':
                    Console.Clear();
                    if (player.checkForMaze())
                    {
                        player.StartPlayerMovement();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Can't enter play mode before generating a maze!");
                        Console.ResetColor();
                        GoBackToMenu();
                        //Thread.Sleep(1250);
                        //ShowMenu();
                    }
                    break;
                case '3':
                    Console.WriteLine("Not yet implemented");
                    break;
                case '4':
                    Console.WriteLine(exitText);
                    return;
            }

        }

        public static void GoBackToMenu()
        {
            Console.WriteLine(backText);
            Console.CursorVisible = true;

            bool valid = false;
            while (!valid)
            {
                char input = Console.ReadLine().ToLower().ToCharArray()[0];
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
                    Console.WriteLine(invalidText);
                }
               
            }
        }
    }
}