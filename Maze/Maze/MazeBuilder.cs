#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0074 // Use compound assignment
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Maze
{
    internal static class MazeBuilder
    {
        private static HuntAndKill maze;
        private static bool hasSet = false;

        public static void SetMazeClass(HuntAndKill huntAndKill)
        {
            maze = huntAndKill;
            hasSet = true;
        }
        
        public static void InitializeMap()
        {
            if (!hasSet) { return; }
            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    maze.ModifyMap(new int[] { i, j }, maze.wallSymbol);
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
                        int randomChance = maze.random.Next(maze.borderRandomizationChance[0], maze.borderRandomizationChance[1]);
                        if (randomChance == maze.borderRandomizationChance[0])
                        {
                            maze.ModifyMap(new int[] { i, j }, maze.blankSymbol);
                        }
                        else
                        {
                            maze.ModifyMap(new int[] { i, j }, maze.wallSymbol);
                        }
                    }
                }
            }
        }

        public static void WriteMap()
        {
            if (!hasSet) { return; }

            for (int i = 0; i < maze.gameMap.GetLength(0); i++)
            {
                Console.SetCursorPosition(0, i);
                for (int j = 0; j < maze.gameMap.GetLength(1); j++)
                {
                    if (maze.coloredOutput)
                    {
                        if (maze.gameMap[i, j] == maze.blankSymbol.ToString())
                        {
                            Console.BackgroundColor = maze.outputColors[0];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i, j] == maze.wallSymbol.ToString())
                        {
                            Console.BackgroundColor = maze.outputColors[1];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (maze.gameMap[i,j] == maze.playerSymbol.ToString())
                        {
                            Console.BackgroundColor = maze.outputColors[2];
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
            if (maze.showProgress && !maze.hasEnded)
            {
                Console.WriteLine("\r\nGenerating maze");
                Console.WriteLine("Note: The maze will generate slower due to showProgress being on.");
                Console.WriteLine("For best results, disable showProgress.");
            }
        }
    }
}