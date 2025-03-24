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
            huntKillMaze.GenerateMaze();
        }
    }
    
}

