namespace MazeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            MazeGenerator mazegen = new MazeGenerator(40, 20);
            mazegen.GenerateMaze();
            mazegen.PrintMaze();
        }
    }
}