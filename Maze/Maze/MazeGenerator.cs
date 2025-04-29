using System.Diagnostics;
using System.Diagnostics.SymbolStore;

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0079 // Remove unnecessary suppresion
#pragma warning disable CA1822  // Mark members as static
#pragma warning disable IDE0028 // Simplify colllection initialization
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Maze
{
    internal class MazeGenerator
    {
        /* ---------------------------------------------------------------------------------- */

        // GAME MAP & COORDINATES
        public string[,] gameMap { get; private set; }
        private int[] currentCoords = new int[] { 1, 1 };
        public bool isPreloading = false;
        private ConfigData config;

        // GAME STATE
        public bool hasEnded { get; private set; } = false;

        // UTILITY
        public Random random { get; private set; } = new Random();
        private Stopwatch stopwatch = new Stopwatch();

        /* ---------------------------------------------------------------------------------- */

        public MazeGenerator()
        {
            ConfigData config = new ConfigData();
            this.config = config;

            MazePlayer player = new MazePlayer(this, config);
            MazeBuilder.Setup(this, config);
            MainMenu.Setup(this, player);

            Preload();
            MainMenu.ShowMenu();
        }

        /// <summary>
        /// If pos in gameMap is a wall, return true
        /// </summary>
        private bool isWall(int[] pos)
        {
            return (gameMap[pos[0], pos[1]] == config.GetValue<char>("wallSymbol").ToString());
        }

        public void ModifyMap(int[] pos, object symbol)
        {
            gameMap[pos[0], pos[1]] = symbol.ToString();
        }

        /// <summary>
        /// If pos is within the bounds of gameMap, return true
        /// </summary>
        private bool isWithinBounds(int[] pos)
        {
            return pos[0] >= 0 && pos[0] < gameMap.GetLength(0) && pos[1] >= 0 && pos[1] < gameMap.GetLength(1);
        }

        /// <summary>
        /// Carves a two step path in the chosen direction
        /// </summary>
        private void CarvePath(int[] direction, int[] direction2)
        {
            gameMap[direction[0], direction[1]] = config.GetValue<char>("blankSymbol").ToString();
            gameMap[direction2[0], direction2[1]] = config.GetValue<char>("blankSymbol").ToString();
        }

        /// <summary>
        /// Collects an array of all the available directions from the current position pos and returns it.
        /// </summary>
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

        /// <summary>
        /// Checks if a direction is valid and adds it to the list of availableDirections.
        /// A direction is valid two cells in that direction are within bounds and meet the wall conditions.
        /// If requiresWall is true, both cells in that direction must be walls.
        /// Otherwise the second cell must not be a wall, but the first cell can be anything.
        /// </summary>
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

        /// <summary>
        /// Getter to prevent access to variables of StartPreloading
        /// </summary>
        public void Preload()
        {
            StartPreloading();
        }

        /// <summary>
        /// Preloads the maze generation to allow for faster generation
        /// </summary>
        private void StartPreloading()
        {
            string[,] tempArray = gameMap;
            gameMap = config.GetValue<string[,]>("preloadGameMap");

            isPreloading = true;
            GenerateMaze();
            gameMap = tempArray;
            isPreloading = false;
        }

        /// <summary>
        /// Getter to prevent access to variables of InitializeMaze
        /// </summary>
        public void GenerateMaze()
        {
            hasEnded = false;
            if (!isPreloading) { gameMap = config.GetValue<string[,]>("gameMap"); }
            MazeBuilder.InitializeMap(); // Initializes gameMap by filling it with walls
            if (config.GetValue<bool>("measureSpeed"))
            {
                stopwatch.Reset(); 
                stopwatch.Start();
            }
            else
            {
                Console.WriteLine("Generating maze");
            }
            Console.CursorVisible = false;
            if (gameMap.GetLength(0) >= Console.BufferHeight)
            {
                Console.SetBufferSize(Console.BufferWidth, gameMap.GetLength(0));
            }
            if (gameMap.GetLength(1) >= Console.BufferWidth)
            {
                Console.SetBufferSize(gameMap.GetLength(1), Console.BufferHeight);
            }
            Console.WriteLine(Console.BufferHeight + " " + Console.BufferWidth);

            currentCoords = config.GetValue<int[]>("startCoords");
            if (isWithinBounds(new int[] { Math.Abs(currentCoords[0]), Math.Abs(currentCoords[1]) }))
            {
                gameMap[Math.Abs(currentCoords[0]), Math.Abs(currentCoords[1])] = config.GetValue<char>("blankSymbol").ToString(); // Makes the start position blank
                Kill();
            }
            else
            {
                throw new InvalidOperationException("startCoords is not within gameMap");
            }
            
        }

        /// <summary>
        /// Searches from left to right after a cell with available neighbours and then continues to kill from the new cell
        /// </summary>
        private void Hunt()
        {
            // Loops through the gameMap from left to right
            for (int i = 0; i < gameMap.GetLength(0); i++)
            {
                for (int j = 0; j < gameMap.GetLength(1); j++)
                {
                    // If i and j are odd and are represented by # on gameMap
                    if (gameMap[i, j] == config.GetValue<char>("wallSymbol").ToString() && i % 2 != 0 && j % 2 != 0)
                    {
                        List<int[]> availableDirections = GetAvailableDirections(new int[] { i, j }, false);

                        // If there's available neighbours at i and j, continue, otherwise go to next iteration
                        if (availableDirections.Count > 0)
                        {
                            int randomIndex = random.Next(availableDirections.Count); // Generates a random index of availableDirections
                            int[] chosenDirection = availableDirections[randomIndex]; // The direction in availableDirections chosen by the random index
                            currentCoords = chosenDirection; // Sets the current position to the newly chosen direction
                            gameMap[currentCoords[0], currentCoords[1]] = config.GetValue<char>("blankSymbol").ToString(); // Sets the current position in gameMap to the chosen blank symbol
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

        /// <summary>
        /// Clears and outputs the completed maze as well as showing additional information and randomizes the borders if set
        /// </summary>
        private void EndGeneration()
        {
            hasEnded = true;
            Console.Clear();

            if (!isPreloading)
            {
                if (config.GetValue<bool>("measureSpeed") && stopwatch.IsRunning) { stopwatch.Stop(); } // Stops the stopwatch if measureSpeed is true and a stopwatch instance is running
                if (config.GetValue<bool>("doRandomizeBorders")) { MazeBuilder.RandomizeBorders(); } // If doRandomizeBorders, wait for RandomizeBorders to finish randomizing the borders of gameMap
                MazeBuilder.WriteMap(false);

                Console.WriteLine("\r\nGeneration complete.");

                if (config.GetValue<bool>("measureSpeed") && stopwatch.Elapsed != TimeSpan.Zero) // If measureSpeed is true and stopwatch.Elapsed is not 0, aka it has recorded the time correctly, output the speed information
                {
                    Console.WriteLine("\r\n----- Speed measurement -----");
                    Console.WriteLine("Seconds: " + Math.Round(stopwatch.Elapsed.TotalSeconds, 3));
                    Console.WriteLine("Milliseconds: " + Math.Round(stopwatch.Elapsed.TotalMilliseconds, 3) + "\r\n");
                }

                MainMenu.GoBackToMenu();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Kill()
        {
            int delay = config.GetValue<int>("delay");
            if (delay != 0) { Thread.Sleep(delay); }
            if (config.GetValue<bool>("showProgress") && !isPreloading) { MazeBuilder.WriteMap(); }

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