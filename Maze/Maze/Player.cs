#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0290 // Use primary constructor

namespace Maze
{
    internal class Player
    {
        private int playerX, playerY;
        public bool isPlaying { get; private set; } = false;
        private readonly HuntAndKill maze;

        public Player(HuntAndKill huntAndKill)
        {
            maze = huntAndKill;
        }

        public void StartPlayerMovement()
        {
            Console.CursorVisible = false;
            if (isValid(maze.startCoords[0], maze.startCoords[1]))
            {
                maze.ModifyMap(new int[] { maze.startCoords[0], maze.startCoords[1] }, maze.playerSymbol);
                playerY = maze.startCoords[0];
                playerX = maze.startCoords[1];
           
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
            return (y >= 0 && y < maze.gameMap.GetLength(0) && x >= 0 && x < maze.gameMap.GetLength(1) && maze.gameMap[y, x] == maze.blankSymbol.ToString());
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
                                maze.ModifyMap(new int[] { playerY, playerX }, maze.blankSymbol);
                                maze.ModifyMap(new int[] { playerY, xLeft}, maze.playerSymbol);
                                playerX = xLeft;
                                MazeBuilder.WriteMap();
                            }
                            break;
                        case ConsoleKey.D:
                            int xRight = playerX + 1;
                            if (isValid(playerY, xRight))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, maze.blankSymbol);
                                maze.ModifyMap(new int[] { playerY, xRight }, maze.playerSymbol);
                                playerX = xRight;
                                MazeBuilder.WriteMap();
                            }
                            break;
                        case ConsoleKey.W:
                            int yUp = playerY - 1;
                            if (isValid(yUp, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, maze.blankSymbol);
                                maze.ModifyMap(new int[] { yUp, playerX }, maze.playerSymbol);
                                playerY = yUp;
                                MazeBuilder.WriteMap();
                            }
                            break;
                        case ConsoleKey.S:
                            int yDown = playerY + 1;
                            if (isValid(yDown, playerX))
                            {
                                maze.ModifyMap(new int[] { playerY, playerX }, maze.blankSymbol);
                                maze.ModifyMap(new int[] { yDown, playerX }, maze.playerSymbol);
                                playerY = yDown;
                                MazeBuilder.WriteMap();
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
