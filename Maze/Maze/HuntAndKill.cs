using System.Diagnostics;

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0079 // Remove unnecessary suppresion
#pragma warning disable CA1822  // Mark members as static
#pragma warning disable IDE0028 // Simplify colllection initialization
#pragma warning disable IDE0090 // Use 'new(...)''
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0044 // Add readonly modifier

namespace Maze
{
    internal class HuntAndKill
    {
        /* ---------------------------------------------------------------------------------- */

        // GAME MAP & COORDINATES
        private string[,] gameMap = new string[21, 69];
        private int[] currentCoords = new int[] { 1, 1 };

        // GAME STATE
        private bool hasEnded = false;
        private bool isActive = false;

        // CONFIGURATION
        private int delay = 0;
        private bool doRandomizeBorders = false;
        private bool coloredOutput = true;
        private bool showProgress = false;
        private bool measureSpeed = true;
        private char wallSymbol = '#';
        private char blankSymbol = ' ';
        private ConsoleColor[] outputColors = new ConsoleColor[] { ConsoleColor.Black, ConsoleColor.DarkGray };

        // RANDOMIZATION CHANCES
        private int[] randomizeBordersChance = new int[] { 1, 8 };

        // UTILITY
        private Random random = new Random();
        private Stopwatch stopwatch = new Stopwatch();

        /* ---------------------------------------------------------------------------------- */

        // If pos in gameMap is a wall, return true
        private bool isWall(int[] pos)
        {
            return (gameMap[pos[0], pos[1]] == wallSymbol.ToString());
        }

        // If pos is within the bounds of gameMap, return true
        private bool isWithinBounds(int[] pos)
        {
            return pos[0] >= 0 && pos[0] < gameMap.GetLength(0) && pos[1] >= 0 && pos[1] < gameMap.GetLength(1);
        }

        // Carves a two step path in the chosen direction
        private void CarvePath(int[] direction, int[] direction2)
        {
            gameMap[direction[0], direction[1]] = blankSymbol.ToString();
            gameMap[direction2[0], direction2[1]] = blankSymbol.ToString();
        }

        // Collects an array of all the available directions from the current position pos and returns it
        private List<int[]> GetAvailableDirections(int[] pos, bool requiresWall)
        {
            List<int[]> availableDirections = new List<int[]>();
            int i = pos[0];
            int j = pos[1];

            CheckDirection(new int[] { i, j - 2 }, new int[] { i, j - 1 }, availableDirections, requiresWall);
            CheckDirection(new int[] { i, j + 2 }, new int[] { i, j + 1 }, availableDirections, requiresWall);
            CheckDirection(new int[] { i + 2, j }, new int[] { i + 1, j }, availableDirections, requiresWall);
            CheckDirection(new int[] { i - 2, j }, new int[] { i - 1, j }, availableDirections, requiresWall);

            return availableDirections;
        }

        // Checks if a direction is valid and adds it to the list of availableDirections
        // A direction is valid two cells in that direction are within bounds and meet the wall conditions
        // If requiresWall is true, both cells in that direction must be walls
        // Otherwise the second cell must not be a wall, but the first cell can be anything
        private void CheckDirection(int[] direction, int[] direction2, List<int[]> availableDirections, bool requiresWall)
        {
            if (requiresWall)
            {
                if (isWithinBounds(new int[] { direction[0], direction[1] }) && isWithinBounds(new int[] { direction2[0], direction2[1] }) &&
                    isWall(new int[] { direction[0], direction[1] }) && isWall(new int[] { direction2[0], direction2[1] }))
                {
                    availableDirections.Add(new int[] { direction[0], direction[1] });
                }
            }
            else
            {
                if (isWithinBounds(new int[] { direction[0], direction[1] }) && isWithinBounds(new int[] { direction2[0], direction2[1] }) &&
                    !isWall(new int[] { direction[0], direction[1] }))
                {
                    availableDirections.Add(new int[] { direction[0], direction[1] });
                }
            }
        }

        // Not yet implemented
        public void SetConfig()
        {

        }

        /// <summary>
        /// Initializing and commencing the maze generation 
        /// </summary>
        public void GenerateMaze()
        {
            if (isActive) { return; } // Returns if the program is already generating a maze

            isActive = true;
            hasEnded = false; // Ensures hasEnded is always false when generating a new maze
            Console.CursorVisible = false; // Hides the cursor
            MazeBuilder.InitializeMap(gameMap, wallSymbol); // Initializes gameMap by filling it with walls

            // Experimental
            //Console.SetWindowSize(gameMap.GetLength(1), gameMap.GetLength(0));
            //Console.SetBufferSize(gameMap.GetLength(1), gameMap.GetLength(0));

            if (!showProgress) { Console.WriteLine("Generating maze"); }
            if (measureSpeed) { stopwatch.Start(); } // Starts the timer if measureSpeed is true

            gameMap[currentCoords[0], currentCoords[1]] = blankSymbol.ToString(); // Makes the start position blank
            Kill(); 
        }

        // Searches from left to right after a cell with available neighbours and then continues to kill from the new cell
        private void Hunt()
        {
            if (!isActive) { return; }

            // Loops through the gameMap from left to right
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    // If i and j are odd and are represented by # on gameMap
                    if (gameMap[i, j] == wallSymbol.ToString() && i % 2 != 0 && j % 2 != 0)
                    {
                        List<int[]> availableDirections = GetAvailableDirections(new int[] { i, j }, false);

                        // If there's available neighbours at i and j, continue, otherwise go to next iteration
                        if (availableDirections.Count > 0)
                        {
                            int randomIndex = random.Next(availableDirections.Count); // Generates a random index of availableDirections
                            int[] chosenDirection = availableDirections[randomIndex]; // The direction in availableDirections chosen by the random index
                            currentCoords = chosenDirection; // Sets the current position to the newly chosen direction
                            gameMap[currentCoords[0], currentCoords[1]] = blankSymbol.ToString(); // Sets the current position in gameMap to the chosen blank symbol
                            Kill(); // Continue the maze generation
                            return; // Stops the for loops
                        }
                    }
                    // End of maze generation, no more available cells were found
                    else if (i == gameMap.GetLength(0) - 1 && j == gameMap.GetLength(1) - 1)
                    {
                        EndGeneration();
                    }
                }
            }
        }

        // Clears and outputs the completed maze as well as showing additional information and randomizes the borders if set
        private async void EndGeneration()
        {
            hasEnded = true;
            Console.Clear();

            if (measureSpeed && stopwatch.IsRunning) { stopwatch.Stop(); } // Stops the stopwatch if measureSpeed is true and a stopwatch instance is running
            if (doRandomizeBorders) { await MazeBuilder.RandomizeBorders(gameMap,random, randomizeBordersChance, wallSymbol, blankSymbol); } // If doRandomizeBorders, wait for RandomizeBorders to finish randomizing the borders of gameMap
            MazeBuilder.WriteMap(gameMap, coloredOutput, showProgress, hasEnded, outputColors, wallSymbol, blankSymbol);

            Console.WriteLine("\r\nGeneration complete.\r\n");
            isActive = false;

            if (measureSpeed && stopwatch.Elapsed != TimeSpan.Zero) // If measureSpeed is true and stopwatch.Elapsed is not 0, aka it has recorded the time correctly, output the speed information
            {
                Console.WriteLine("----- Speed measurement -----");
                Console.WriteLine("Seconds: " + Math.Round(stopwatch.Elapsed.TotalSeconds, 3));
                Console.WriteLine("Milliseconds: " + Math.Round(stopwatch.Elapsed.TotalMilliseconds, 3));
            }
        }

        private void Kill()
        {
            if (!isActive) { return; }
            if (delay > 0) { Thread.Sleep(delay); }
            if (showProgress) { MazeBuilder.WriteMap(gameMap, coloredOutput, showProgress, hasEnded, outputColors, wallSymbol, blankSymbol); }

            List<int[]> availableDirections = GetAvailableDirections(new int[] { currentCoords[0], currentCoords[1] }, true); // Gets the neighbours of the current coordinates that are walls and not out of bounds

            // There are available directions
            if (availableDirections.Count > 0)
            {
                int randomIndex = random.Next(availableDirections.Count); // Generates a random number that corresponds to an index of availableDirecitons
                int[] chosenDirection = availableDirections[randomIndex]; // The direction in availableDirections chosen by the random index

                if (chosenDirection[0] == currentCoords[0] + 2)
                {
                    CarvePath(new int[] { chosenDirection[0] - 1, chosenDirection[1] }, new int[] { chosenDirection[0], chosenDirection[1] });
                }
                else if (chosenDirection[0] == currentCoords[0] - 2)
                {
                    CarvePath(new int[] { chosenDirection[0] + 1, chosenDirection[1] }, new int[] { chosenDirection[0], chosenDirection[1] });
                }
                else if (chosenDirection[1] == currentCoords[1] + 2)
                {
                    CarvePath(new int[] { chosenDirection[0], chosenDirection[1] - 1 }, new int[] { chosenDirection[0], chosenDirection[1] });
                }
                else if (chosenDirection[1] == currentCoords[1] - 2)
                {
                    CarvePath(new int[] { chosenDirection[0], chosenDirection[1] + 1 }, new int[] { chosenDirection[0], chosenDirection[1] });
                }

                currentCoords = chosenDirection;

                Kill(); // Continues the kill phase
            }
            // There are no available directions
            else
            {
                Hunt(); // Switch to hunt mode
            }
        }
    }
}
