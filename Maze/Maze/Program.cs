// TODO V2:
// Add more comments
// Add interactive config
// Improve randomize border system

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable IDE0090 // Use 'new(...)'

namespace Maze
{
    internal class Program
    {
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
        static void Main()
        {
            HuntAndKill huntKillMaze = new HuntAndKill();
            MazeBuilder.SetMazeClass(huntKillMaze);
            huntKillMaze.PreloadGeneration();
            huntKillMaze.GenerateMaze();

            //Generate new maze question
            while (true)
            {
                Console.WriteLine("Would you like to generate a new maze?");
                Console.CursorVisible = true;
                string input = Console.ReadLine().ToLower();
                if (input == "y" || input == "yes")
                {
                    Console.Clear();
                    huntKillMaze.GenerateMaze();
                }
                else
                {
                    return;
                }
            }

            // Message box question
            //int input = MessageBox(IntPtr.Zero, "Would you like to walk through the maze?", "Maze", 4);
            //Console.CursorVisible = true;
            //if (input == 6)
            //{
            //    Player player = new Player(huntKillMaze.gameMap, huntKillMaze.blankSymbol);
            //    player.StartPlayerMovement(huntKillMaze.startCoords[0], huntKillMaze.startCoords[1], huntKillMaze);
            //}

            //Movement system question
            //Console.CursorVisible = true;
            //Console.WriteLine("Would you like to walk through the maze?");

            //string input = Console.ReadLine().ToLower();

            //if (input == "yes" || input == "y")
            //{
            //    Player player = new Player(huntKillMaze);
            //    player.StartPlayerMovement();
            //}

        }
    }
    
}

