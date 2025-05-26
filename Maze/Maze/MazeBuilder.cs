#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0074 // Use compound assignment
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Maze
{
    internal static class MazeBuilder
    {
        private static MazeGenerator maze;

        public static void Setup(MazeGenerator huntAndKill)
        {
            maze = huntAndKill;
        }
        
        /// <summary>
        /// Fills the gameMap with wallSymbols (default: '#')
        /// </summary>
        public static void InitializeMap()
        {
            if (maze == null || maze.gameMap == null) { return; }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    maze.ModifyMap(new int[] { i, j }, ConfigData.GetValue<char>("wallSymbol").ToString());
                }
            }
        }

        /// <summary>
        /// Randomizes each part of the border to allow for entrances and exits
        /// </summary>
        public static void RandomizeBorders()
        {
            if (maze == null || maze.gameMap == null) { return; }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    if (i == 0 || i == maze.gameMap.GetLength(0) - 1 || j == 0 || j == maze.gameMap.GetLength(1) - 1)
                    {
                        int[] randomizationChance = ConfigData.GetValue<int[]>("borderRandomizationChance");
                        int randomChance = maze.random.Next(randomizationChance[0], randomizationChance[1]);

                        string symbol = (randomChance == randomizationChance[0]) ? ConfigData.GetValue<char>("blankSymbol").ToString() : ConfigData.GetValue<char>("wallSymbol").ToString();
                        maze.ModifyMap(new int[] { i, j }, symbol);
                    }
                }
            }
        }

        /// <summary>
        /// Outputs the contents of gameMap, in color or with symbols depending on the config
        /// </summary>
        public static void WriteMap(bool isWalking = false)
        {
            if (maze == null || maze.gameMap == null) { return; }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                Console.SetCursorPosition(0, i);
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    if (ConfigData.GetValue<bool>("coloredOutput"))
                    {
                        ConsoleColor[] outputColors = ConfigData.GetValue<ConsoleColor[]>("outputColors");
                        if (maze.gameMap[i, j] == ConfigData.GetValue<char>("blankSymbol").ToString())
                        {
                            Console.BackgroundColor = outputColors[0];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i, j] == ConfigData.GetValue<char>("wallSymbol").ToString())
                        {
                            Console.BackgroundColor = outputColors[1];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i,j] == ConfigData.GetValue<char>("playerSymbol").ToString())
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
            if (!maze.hasEnded)
            {
                Console.WriteLine("\r\nGenerating maze...");
                if (ConfigData.GetValue<bool>("showProgress"))
                {
                    Console.WriteLine("Maze generator will generate slower due to showProgress being on.");
                    Console.WriteLine("For best results, disable showProgress.");
                }
            }
            
        }
    }
}