#pragma warning disable IDE0300
#pragma warning disable IDE0074

using System;
using System.Security;

namespace Maze
{
    internal static class MazeBuilder
    {
        public static void InitializeMap(string[,] gameMap, char wallSymbol)
        {
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    gameMap[i, j] = wallSymbol.ToString();
                }
            }
        }

        // Randomizes each part of the border to allow for entrances and exits
        public static Task RandomizeBorders(string[,] gameMap, Random random, int[] randomizeBordersChance, char wallSymbol, char blankSymbol)
        {
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    if (i == 0 || i == gameMap.GetLength(0) - 1 || j == 0 || j == gameMap.GetLength(1) - 1)
                    {
                        int randomChance = random.Next(randomizeBordersChance[0], randomizeBordersChance[1]);
                        if (randomChance == randomizeBordersChance[0])
                        {
                            gameMap[i, j] = blankSymbol.ToString();
                        }
                        else
                        {
                            gameMap[i, j] = wallSymbol.ToString();
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static void WriteMap(string[,] gameMap, bool coloredOutput = false, bool showProgress = false, bool hasEnded = false, ConsoleColor[]? outputColors = null, char wallSymbol = '#', char blankSymbol = ' ')
        {
            if (outputColors == null) { outputColors = new ConsoleColor[] { ConsoleColor.White, ConsoleColor.White }; } // Sets default value for outputColors

            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                Console.SetCursorPosition(0, i);
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    if (coloredOutput)
                    {
                        if (gameMap[i, j] == blankSymbol.ToString())
                        {
                            Console.BackgroundColor = outputColors[0];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                        else if (gameMap[i, j] == wallSymbol.ToString())
                        {
                            Console.BackgroundColor = outputColors[1];
                            Console.Write(" ");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write(gameMap[i, j]);
                    }
                }
            }
            if (showProgress && !hasEnded)
            {
                Console.WriteLine("\r\nGenerating maze");
                Console.WriteLine("Note: The maze will generate slower due to showProgress being on.");
                Console.WriteLine("For best results, disable showProgress.");
            }
        }
    }
}