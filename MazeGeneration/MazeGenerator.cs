using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGeneration
{
    internal class MazeGenerator
    {
        private readonly Random RD = new Random();
        public const int Wall = 1;
        public const int Path = 0;

        private readonly int SizeOfMazeX;
        private readonly int SizeOfMazeY;
        private int CurrentLocationX = 1;
        private int CurrentLocationY = 1;

        private enum Direction
        { Up, Right, Down, Left }

        private Direction CurrentDirection = Direction.Right;

        private List<List<int>> Maze { get; set; } = new List<List<int>> { };

        public MazeGenerator(int MazeX, int MazeY)
        {
            // for maze to be have odd x and y values. ensures wall all around maze.
            if (MazeX % 2 != 0) { SizeOfMazeX = MazeX; }
            else { SizeOfMazeX = MazeX + 1; }

            if (MazeY % 2 != 0) { SizeOfMazeY = MazeY; }
            else { SizeOfMazeY = MazeY + 1; }
        }

        public List<List<int>> GenerateMaze()
        {
            // generates a blank maze - i.e. one with no paths
            Maze = InitaliseMaze(SizeOfMazeX, SizeOfMazeY);

            // Move through maze creating paths from random starting point.
            // Paths must be contained within the maze ie must have wall all around maze.
            // There must be at least one wall tile between paths.

            List<List<int>> Stack = new List<List<int>> { }; // List of visited tiles that contain more than one possible direction of travel

            while (true)
            {
                Maze[CurrentLocationY][CurrentLocationX] = Path;
                var PossibleDirectionsToTravel = ChooseDirection(); // returns valid directions
                if (PossibleDirectionsToTravel.Any())
                {
                    if (PossibleDirectionsToTravel.Count > 1) { Stack.Add(new List<int> { CurrentLocationX, CurrentLocationY }); }
                    CurrentDirection = PossibleDirectionsToTravel[RD.Next(PossibleDirectionsToTravel.Count)]; // choose direction at random
                    UpdatePosistion();
                    // as we move 2 squares due to wall.
                    Maze[CurrentLocationY][CurrentLocationX] = Path;
                    UpdatePosistion();
                }
                else
                {
                    if (Stack.Any())
                    {
                        // Path generator has created a local dead end.
                        // Itterate backwards through stack looking for a location that has a vaild move and continue path generation from there.
                        CurrentLocationX = Stack[Stack.Count - 1][0];
                        CurrentLocationY = Stack[Stack.Count - 1][1];
                        Stack.RemoveAt(Stack.Count - 1);
                    }
                    else
                    {
                        // if no vail location to branch off into new path stop.
                        break;
                    }
                }
            }

            PlaceEntrances();

            return Maze;
        }

        private void PlaceEntrances()
        {
            // Scans either the horiznal or vertical and determins if there is a ajacent path to this locaion and adds to list.
            // returns a random int that repsents the chosesn location from this vaild list.

            String[] Edge = new string[] { "Top", "Right", "Bottom", "Left" };
            List<String> Entrances = new List<string> { };
            do
            {
                string edge = Edge[RD.Next(0, Edge.Count())];
                if (!Entrances.Contains(edge)) { Entrances.Add(edge); }
            }
            while (Entrances.Count < 2);

            foreach (var entrance in Entrances)
            {
                AddEntrance(entrance);
            }
        }

        private void AddEntrance(string entrance)
        {
            List<int> Locations = new List<int> { };
            switch (entrance)
            {
                case ("Top"):

                    for (int Column = 1; Column < SizeOfMazeX - 1; Column++)
                    {
                        if (Maze[1][Column] == 0 && Maze[0][Column] != 0) { Locations.Add(Column); }
                    }

                    Maze[0][Locations[RD.Next(0, Locations.Count)]] = Path;
                    break;

                case ("Bottom"):

                    for (int Column = 1; Column < SizeOfMazeX - 1; Column++)
                    {
                        if (Maze[SizeOfMazeY - 2][Column] == 0 && Maze[SizeOfMazeY - 1][Column] != 0) { Locations.Add(Column); }
                    }

                    Maze[SizeOfMazeY - 1][Locations[RD.Next(0, Locations.Count)]] = Path;
                    break;

                case ("Left"):

                    for (int Row = 1; Row < SizeOfMazeY - 1; Row++)
                    {
                        if (Maze[Row][1] == 0 && Maze[Row][0] != 0) { Locations.Add(Row); }
                    }

                    Maze[Locations[RD.Next(0, Locations.Count)]][0] = Path;
                    break;

                case ("Right"):

                    for (int Row = 1; Row < SizeOfMazeY - 1; Row++)
                    {
                        if (Maze[Row][SizeOfMazeY - 2] == 0 && Maze[Row][SizeOfMazeY - 1] != 0) { Locations.Add(Row); }
                    }

                    Maze[Locations[RD.Next(0, Locations.Count)]][SizeOfMazeX - 1] = Path;
                    break;
            }
        }

        private List<Direction> ChooseDirection()
        {
            Direction[] Directions = { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
            List<Direction> ValidDirections = new List<Direction> { };

            foreach (var direction in Directions)
            {
                int a = CurrentLocationX, b = CurrentLocationY;
                switch (direction)
                {
                    // The increase in a and b by two ensures that a wall tile is left between paths
                    case (Direction.Up):
                        b -= 2;
                        break;

                    case (Direction.Right):
                        a += 2;
                        break;

                    case (Direction.Down):
                        b += 2;
                        break;

                    case (Direction.Left):
                        a -= 2;
                        break;
                }
                if (a > 0 && a < SizeOfMazeX - 1 && b > 0 && b < SizeOfMazeY - 1 && Maze[b][a] == 1) { ValidDirections.Add(direction); }
            }
            return ValidDirections;
        }

        private void UpdatePosistion()
        {
            switch (CurrentDirection)
            {
                case (Direction.Up):
                    CurrentLocationY--;
                    break;

                case (Direction.Right):
                    CurrentLocationX++;
                    break;

                case (Direction.Down):
                    CurrentLocationY++;
                    break;

                case (Direction.Left):
                    CurrentLocationX--;
                    break;
            }
        }

        private static List<List<int>> InitaliseMaze(int SizeX, int SizeY)
        {
            List<List<int>> BlankMaze = new List<List<int>> { };
            for (int row = 0; row < SizeY; row++)
            {
                List<int> Row = new List<int> { };
                for (int col = 0; col < SizeX; col++)
                {
                    Row.Add(Wall);
                }
                BlankMaze.Add(Row);
            }

            return BlankMaze;
        }

        public void PrintMaze()
        {
            for (int row = 0; row < Maze.Count; row++)
            {
                for (int col = 0; col < Maze[row].Count; col++)
                {
                    if (Maze[row][col] == 0) { Console.Write(" "); }
                    if (Maze[row][col] == 1) { Console.Write("#"); }
                    if (Maze[row][col] == 2) { Console.Write("."); }
                }
                Console.WriteLine();
            }
        }
    }
}