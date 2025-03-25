using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    internal class Player
    {
        private int playerX, playerY;
        private string[,] gameMap;
        public bool isPlaying;
        private char blankSymbol;
        public char playerSymbol = 'X';
        public ConsoleColor[] outputColors;
        public bool coloredOutput;

        public Player(string[,] map, char symbol)
        {
            gameMap = map;
            blankSymbol = symbol;
        }

        public void StartPlayerMovement(int startX, int startY, HuntAndKill huntKillMaze)
        {
            outputColors = huntKillMaze.outputColors;
            coloredOutput = huntKillMaze.coloredOutput;
            MazeBuilder.playerSymbol = playerSymbol;
            Console.WriteLine(startX + " " +  startY);
            if (isValid(startY, startX))
            {
                gameMap[startY, startX] = playerSymbol.ToString();
                playerX = startX;
                playerY = startY;
                Console.Clear();
                MazeBuilder.WriteMap(gameMap, coloredOutput, false, false, outputColors);
                isPlaying = true;
                MovePlayer();
            }
            else
            {
                Console.WriteLine("Start pos is not valid, play mode will not start");
            }
        }

        private bool isValid(int y, int x)
        {
            return (y >= 0 && y < gameMap.GetLength(0) && x >= 0 && x < gameMap.GetLength(1) && gameMap[y, x] == blankSymbol.ToString());
        }

        private void MovePlayer()
        {
            while (isPlaying)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.A:
                            int xLeft = playerX - 1;
                            if (isValid(playerY, xLeft))
                            {
                                gameMap[playerY, playerX] = blankSymbol.ToString();
                                gameMap[playerY, xLeft] = playerSymbol.ToString();
                                playerX = xLeft;
                                Console.Clear();
                                MazeBuilder.WriteMap(gameMap, coloredOutput, false, false, outputColors);
                            }
                            break;
                        case ConsoleKey.D:
                            int xRight = playerX + 1;
                            if (isValid(playerY, xRight))
                            {
                                gameMap[playerY, playerX] = blankSymbol.ToString();
                                gameMap[playerY, xRight] = playerSymbol.ToString();
                                playerX = xRight;
                                Console.Clear();
                                MazeBuilder.WriteMap(gameMap, coloredOutput, false, false, outputColors);
                            }
                            break;
                        case ConsoleKey.W:
                            int yUp = playerY - 1;
                            if (isValid(yUp, playerX))
                            {
                                gameMap[playerY, playerX] = blankSymbol.ToString();
                                gameMap[yUp, playerX] = playerSymbol.ToString();
                                playerY = yUp;
                                Console.Clear();
                                MazeBuilder.WriteMap(gameMap, coloredOutput, false, false, outputColors);
                            }
                            break;
                        case ConsoleKey.S:
                            int yDown = playerY + 1;
                            if (isValid(yDown, playerX))
                            {
                                gameMap[playerY, playerX] = blankSymbol.ToString();
                                gameMap[yDown, playerX] = playerSymbol.ToString();
                                playerY = yDown;
                                Console.Clear();
                                MazeBuilder.WriteMap(gameMap, coloredOutput, false, false, outputColors);
                            }
                            break;
                        case ConsoleKey.Spacebar:
                            Console.WriteLine("\r\nYou exited out of play mode");
                            isPlaying = false;
                            break;
                    }
                }
            }
        }
    }
}
