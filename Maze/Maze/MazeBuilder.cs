#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0074 // Use compound assignment
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using System.Reflection.PortableExecutable;

namespace Maze
{
    internal static class MazeBuilder
    {
        private static HuntAndKill maze;
        private static ConfigData config;
        private static bool hasSet = false;

        public static void SetMazeClass(HuntAndKill huntAndKill, ConfigData Config)
        {
            maze = huntAndKill;
            config = Config;
            hasSet = true;
        }
        
        public static void InitializeMap()
        {
            if (!hasSet) { return; }
            if (maze == null || maze.gameMap == null) { throw new NullReferenceException("maze or gameMap is null"); }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    maze.ModifyMap(new int[] { i, j }, config.GetValue<char>("wallSymbol").ToString());
                }
            }
        }

        // Randomizes each part of the border to allow for entrances and exits
        public static void RandomizeBorders()
        {
            if (!hasSet) { return; }
            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    if (i == 0 || i == maze.gameMap.GetLength(0) - 1 || j == 0 || j == maze.gameMap.GetLength(1) - 1)
                    {
                        int[] randomizationChance = config.GetValue<int[]>("borderRandomizationChance");
                        int randomChance = maze.random.Next(randomizationChance[0], randomizationChance[1]);

                        string symbol = (randomChance == randomizationChance[0]) ? config.GetValue<char>("blankSymbol").ToString() : config.GetValue<char>("wallSymbol").ToString();
                        maze.ModifyMap(new int[] { i, j }, symbol);
                    }
                }
            }
        }

        public static void WriteMap(bool isWalking = false)
        {
            if (!hasSet) { return; }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                Console.SetCursorPosition(0, i);
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    if (config.GetValue<bool>("coloredOutput"))
                    {
                        ConsoleColor[] outputColors = config.GetValue<ConsoleColor[]>("outputColors");
                        if (maze.gameMap[i, j] == config.GetValue<char>("blankSymbol").ToString())
                        {
                            Console.BackgroundColor = outputColors[0];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i, j] == config.GetValue<char>("wallSymbol").ToString())
                        {
                            Console.BackgroundColor = outputColors[1];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i,j] == config.GetValue<char>("playerSymbol").ToString())
                        {
                            Console.BackgroundColor = outputColors[2];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write(maze.gameMap[i, j]);
                    }
                }
            }
            if (isWalking)
            {
                Console.WriteLine("\r\nPress space to exit out of the maze.");
            }
            if (config.GetValue<bool>("showProgress") && !maze.hasEnded)
            {
                Console.WriteLine("\r\nGenerating maze...");
                Console.WriteLine("Note: The maze will generate slower due to showProgress being on.");
                Console.WriteLine("For best results, disable showProgress.");
            }
        }
    }
}