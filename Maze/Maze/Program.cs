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
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MazeGenerator maze = new MazeGenerator();
        }
    }
    
}