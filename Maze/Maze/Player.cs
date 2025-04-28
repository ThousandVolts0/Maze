#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0290 // Use primary constructor
#pragma warning disable CS8600
#pragma warning disable CS8604

namespace Maze
{
    internal class Player
    {
        private int playerX, playerY;
        public bool isPlaying { get; private set; } = false;
        private ConfigData config;
        private readonly HuntAndKill maze;

        public Player(HuntAndKill huntAndKill, ConfigData config)
        {
            this.config = config;
            maze = huntAndKill;
        }

        public void StartPlayerMovement()
        {
            Console.CursorVisible = false;
            int[] startCoords = (int[])config.GetValue("startCoords");
            if (startCoords == null) { throw new NullReferenceException("Startcoords is null"); }

            if (isValid(startCoords[0], startCoords[1]))
            {
                maze.ModifyMap(new int[] { startCoords[0], startCoords[1] }, config.GetValue("playerSymbol"));
                playerY = startCoords[0];
                playerX = startCoords[1];
           
                Console.Clear();
                MazeBuilder.WriteMap();
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
            return (y >= 0 && y < maze.gameMap.GetLength(0) && x >= 0 && x < maze.gameMap.GetLength(1) && maze.gameMap[y, x] == (string)config.GetValue("blankSymbol"));
        }

        private void MovePlayer()
        {
            MovePlayer(config);
        }

        private void MovePlayer(ConfigData config)
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
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xLeft}, config.GetValue("playerSymbol"));
                                playerX = xLeft;
                                
                            }
                            break;
                        case ConsoleKey.D:
                            int xRight = playerX + 1;
                            if (isValid(playerY, xRight))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xRight }, config.GetValue("playerSymbol"));
                                playerX = xRight;
                            }
                            break;
                        case ConsoleKey.W:
                            int yUp = playerY - 1;
                            if (isValid(yUp, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue("blankSymbol"));
                                maze.ModifyMap(new int[] { yUp, playerX }, config.GetValue("playerSymbol"));
                                playerY = yUp;
                            }
                            break;
                        case ConsoleKey.S:
                            int yDown = playerY + 1;
                            if (isValid(yDown, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, config.GetValue("blankSymbol"));
                                maze.ModifyMap(new int[] { yDown, playerX }, config.GetValue("playerSymbol"));
                                playerY = yDown;
                            }
                            break;
                        case ConsoleKey.Spacebar:
                            Console.WriteLine("\r\nYou exited out of play mode.");
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