// TODO V2:
// Add more comments
// Add interactive config
// Improve randomize border system

namespace Maze
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HuntAndKill huntKillMaze = new HuntAndKill();
            huntKillMaze.PreloadGeneration();
            huntKillMaze.GenerateMaze();
            //while (true)
            //{
            //    Console.WriteLine("Would you like to generate a new maze?");
            //    Console.CursorVisible = true;
            //    string input = Console.ReadLine().ToLower();
            //    if (input == "y" || input == "yes")
            //    {
            //        Console.Clear();
            //        huntKillMaze.GenerateMaze();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            Console.WriteLine("Would you like to walk through the maze?");
            Console.CursorVisible = true;
            string input = Console.ReadLine().ToLower();
            if (input == "y" || input == "yes")
            {
                Player player = new Player(huntKillMaze.gameMap, huntKillMaze.blankSymbol);
                player.StartPlayerMovement(huntKillMaze.startCoords[0], huntKillMaze.startCoords[1], huntKillMaze);
            }

        }
    }
    
}

