#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0290 // Use primary constructor
#pragma warning disable CS8600
#pragma warning disable CS8604

namespace Maze
{
    internal class MazePlayer
    {
        private int playerX, playerY;
        public bool isPlaying { get; private set; } = false;
        private readonly ConfigData config;
        private readonly MazeGenerator maze;

        public MazePlayer(MazeGenerator huntAndKill, ConfigData config)
        {
            this.config = config;
            maze = huntAndKill;
        }

        public bool HasGenerated()
        {
            if (maze.gameMap != null)
            {
                return true;
            }
            return false;
        }

        public void StartPlayerMovement()
        {
            if (maze.gameMap == null) { throw new InvalidOperationException("Tried to start play mode before generating a maze!"); }
            Console.CursorVisible = false;
            int[] startCoords = config.GetValue<int[]>("startCoords");

            if (isValid(Math.Abs(startCoords[0]), Math.Abs(startCoords[1])))
            {
                maze.ModifyMap(new int[] { Math.Abs(startCoords[0]), Math.Abs(startCoords[1]) }, config.GetValue<char>("playerSymbol"));
                playerY = Math.Abs(startCoords[0]);
                playerX = Math.Abs(startCoords[1]);
           
                Console.Clear();
                MazeBuilder.WriteMap();
                isPlaying = true;
                ListenForInput();
            }
            else
            {
                throw new InvalidOperationException("Start coords are invalid"); 
            }
        }

        private bool isValid(int y, int x)
        {
            return (y >= 0 && y < maze.gameMap.GetLength(0) && x >= 0 && x < maze.gameMap.GetLength(1) && maze.gameMap[y, x] == config.GetValue<char>("blankSymbol").ToString());
        }

        private void ListenForInput()
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
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xLeft}, config.GetValue<char>("playerSymbol"));
                                playerX = xLeft;
                                
                            }
                            break;
                        case ConsoleKey.D:
                            int xRight = playerX + 1;
                            if (isValid(playerY, xRight))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xRight }, config.GetValue<char>("playerSymbol"));
                                playerX = xRight;
                            }
                            break;
                        case ConsoleKey.W:
                            int yUp = playerY - 1;
                            if (isValid(yUp, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { yUp, playerX }, config.GetValue<char>("playerSymbol"));
                                playerY = yUp;
                            }
                            break;
                        case ConsoleKey.S:
                            int yDown = playerY + 1;
                            if (isValid(yDown, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { yDown, playerX }, config.GetValue<char>("playerSymbol"));
                                playerY = yDown;
                            }
                            break;
                        case ConsoleKey.Spacebar:
                            Console.WriteLine("\r\nYou exited out of play mode.");
                            maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue<char>("blankSymbol"));
                            isPlaying = false;
                            MainMenu.GoBackToMenu();
                            break;
                    }
                }
                MazeBuilder.WriteMap(true);
            }
        }
    }
}