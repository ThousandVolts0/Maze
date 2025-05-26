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
        private readonly MazeGenerator maze;

        public MazePlayer(MazeGenerator huntAndKill)
        {
            maze = huntAndKill;
        }

        /// <summary>
        /// Helper method to check if a maze has been generated, returns true if it has
        /// </summary>
        public bool HasGenerated()
        {
            if (maze.gameMap != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows the user to walk through a generated maze with WASD
        /// </summary>
        public void StartPlayerMovement()
        {
            if (maze.gameMap == null) { throw new InvalidOperationException("Tried to start play mode before generating a maze!"); }

            Console.CursorVisible = false;
            Console.Title = "Play mode - Maze Generator";
            int[] startCoords = ConfigData.GetValue<int[]>("startCoords");

            if (isValid(Math.Abs(startCoords[0]), Math.Abs(startCoords[1]))) // If startCoords are within the bounds of gameMap and is the correct symbol
            {
                // Sets start position to playerSymbol and starts listening for input
                maze.ModifyMap(new int[] { Math.Abs(startCoords[0]), Math.Abs(startCoords[1]) }, ConfigData.GetValue<char>("playerSymbol")); 
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

        /// <summary>
        /// Helper method to check if the coordinates x and y are within the bounds of gameMap and is the correct symbol
        /// </summary>
        private bool isValid(int y, int x)
        {
            return (y >= 0 && y < maze.gameMap.GetLength(0) && x >= 0 && x < maze.gameMap.GetLength(1) && maze.gameMap[y, x] == ConfigData.GetValue<char>("blankSymbol").ToString());
        }

        /// <summary>
        /// Listens for key inputs such as WASD and space, to do different actions
        /// </summary>
        private void ListenForInput()
        {
            while (isPlaying)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.A: // Walk left
                            int xLeft = playerX - 1;
                            if (isValid(playerY, xLeft))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, ConfigData.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xLeft}, ConfigData.GetValue<char>("playerSymbol"));
                                playerX = xLeft;
                                
                            }
                            break;
                        case ConsoleKey.D: // Walk right
                            int xRight = playerX + 1;
                            if (isValid(playerY, xRight))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, ConfigData.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { playerY, xRight }, ConfigData.GetValue<char>("playerSymbol"));
                                playerX = xRight;
                            }
                            break;
                        case ConsoleKey.W: // Walk up
                            int yUp = playerY - 1;
                            if (isValid(yUp, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, ConfigData.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { yUp, playerX }, ConfigData.GetValue<char>("playerSymbol"));
                                playerY = yUp;
                            }
                            break;
                        case ConsoleKey.S: // Walk down
                            int yDown = playerY + 1;
                            if (isValid(yDown, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, ConfigData.GetValue<char>("blankSymbol"));
                                maze.ModifyMap(new int[] { yDown, playerX }, ConfigData.GetValue<char>("playerSymbol"));
                                playerY = yDown;
                            }
                            break;
                        case ConsoleKey.Spacebar: // Exits play mode
                            Console.WriteLine("\r\nYou exited out of play mode.");
                            maze.ModifyMap(new int[] { playerY, playerX }, ConfigData.GetValue<char>("blankSymbol"));
                            isPlaying = false;
                            MainMenu.GoBackToMenu();
                            break;
                    }
                }
                if (isPlaying) { MazeBuilder.WriteMap(true); }
            }
        }
    }
}