using System;
using static System.Console;
using System.Threading;
using NLog;

namespace ConwaysGameOfLife
{
    class Program
    {
        const int Dead = 0;             // Using a grid of 0's and 1's will help us count
        const int Alive = 1;            //   count neighbors efficiently in the Life program.

        static int GridSizeX = 25;
        static int GridSizeY = 25;

        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static string mode;

        static string pattern;

        static int maxGenerations = 50;

        static int fillPercent;

        static void Main(string[] args)
        {
            logger.Info("=== Starting Program ===");
            logger.Info("Parsing arguments...");

            int gridCount = 0;
            int[,] grid = new int[GridSizeX, GridSizeY];
            int[,] nextGrid = new int[GridSizeX, GridSizeY];

            if (args.Length == 0)
            {
                mode = "interactive";
                pattern = "R";
                maxGenerations = -1;
            }
            else if (args.Length == 1)
            {
                if (args[0] == "interactive" || args[0] == "i")
                {
                    mode = "interactive";
                    maxGenerations = -1;
                }
                else if (args[0] == "silent" || args[0] == "s")
                {
                    mode = "silent";
                    maxGenerations = 50;
                }

                pattern = "R";
            }
            else if (args.Length == 2)
            {
                if (args[0] == "interactive" || args[0] == "i")
                {
                    mode = "interactive";
                    maxGenerations = -1;
                }
                else if (args[0] == "silent" || args[0] == "s")
                {
                    mode = "silent";
                    maxGenerations = 50;
                }

                if (args[1] == "R")
                {
                    pattern = "R";
                }
                else if (int.TryParse(args[1], out fillPercent))
                {
                    if (fillPercent >= 0 && fillPercent <= 100)
                    {
                        FillGridRandomly(grid, fillPercent);
                    }
                }
            }
            else if (args.Length == 3)
            {
                if (args[0] == "interactive" || args[0] == "i")
                {
                    mode = "interactive";
                }
                else if (args[0] == "silent" || args[0] == "s")
                {
                    mode = "silent";
                }

                if (args[1] == "R")
                {
                    pattern = "R";
                }
                else if (int.TryParse(args[1], out fillPercent))
                {
                    if (fillPercent >= 0 && fillPercent <= 100)
                    {
                        FillGridRandomly(grid, fillPercent);
                    }
                }

                int.TryParse(args[2], out maxGenerations);
            }

            logger.Info("... argument Parsing complete");
            logger.Info($"{mode} mode");
            if (pattern == "R")
            {
                logger.Info($"starting with: R-Pentamino");
            }
            logger.Info($"running for {maxGenerations} Generations");
            if (pattern == "R")
            {
                logger.Info($"--- Filling grid with R-Pentamino pattern");
                GenerateRPentomino(grid);
            }
            else
            {
                logger.Info($"filling grid randomly at {fillPercent} fill percent");
            }
            bool done = false;
            while (!done)
            {
                logger.Info($"Generation: {gridCount}  aliveCount: {CountLiveCells(grid, GridSizeX, GridSizeY)}");
                if (mode == "interactive")
                {
                    PrintGrid(grid);
                    Thread.Sleep(500);

                    CheckNeighbors(grid, nextGrid);
                    grid = nextGrid;

                    if (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(true).Key;
                        logger.Debug($"{key} pressed...");
                        if (key == ConsoleKey.Q)
                            done = true;
                        else if (key == ConsoleKey.F)
                        {
                            grid = new int[GridSizeX, GridSizeY];
                        }
                        else if (key == ConsoleKey.R)
                        {
                            grid = new int[GridSizeX, GridSizeY];
                        }
                    }
                }
                else if (mode == "silent")
                {

                }

                // Randomly Fill the Grid
                // FillGridRandomly(grid, 20);

                // Display the grid (and log its statistics)
                // WriteLine($"Grid #{gridCount}");
                // PrintGrid(grid);
                // logger.Debug($"Grid #{gridCount}  aliveCount: {CountLiveCells(grid)}");

                // Thread.Sleep(500);

                // Check to see if the user pressed a key
                // if (Console.KeyAvailable)
                // {
                //     ConsoleKey key = Console.ReadKey(true).Key;
                //     logger.Debug($"{key} pressed...");
                //     if (key == ConsoleKey.Q)
                //         done = true;
                //     else if (key == ConsoleKey.F)
                //     {

                //     }
                //     else if (key == ConsoleKey.R)
                //     {

                //     }
                // }

                gridCount++;            // Increment at bottom of loop so that first grid displayed is Grid #0
            }
            logger.Info("=== Ending Program ===");
        }

        static void FillGridRandomly(int[,] grid, int fillPercentage)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    if (RandomBool(fillPercentage) == true)
                        grid[x, y] = Alive;
                    else
                        grid[x, y] = Dead;
                }
            }
        }

        static bool RandomBool(int percent)
        {
            Random rng = new Random();
            return (rng.Next() % 100 < percent);
        }

        static void PrintGrid(int[,] grid)
        {
            WriteLine($"+{Dashes(GridSizeX * 3)}+");
            for (int y = 0; y < GridSizeY; y++)
            {
                string s = "|";
                for (int x = 0; x < GridSizeX; x++)
                {
                    string cell = (grid[x, y] == Alive) ? " * " : "   ";
                    s += cell;
                }
                s += "|";
                WriteLine(s);
            }
            WriteLine($"+{Dashes(GridSizeX * 3)}+");
        }

        static string Dashes(int number)
        {
            return new string('-', number);
        }

        static int CountLiveCells(int[,] grid, int _GridSizeX, int _GridSizeY)
        {
            int count = 0;
            for (int x = 0; x < _GridSizeX; x++)
                for (int y = 0; y < _GridSizeY; y++)
                    if (grid[x, y] == Alive)
                        count++;
            return count;
        }

        static void CheckNeighbors(int[,] currentGrid, int[,] nextGrid)
        {
            int livingNeighbors = 0;

            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    int[,] neighbors = new int[3, 3];
                    //put the neighboring cells into a smaller grid for easier checking
                    if (x - 1 > 0)
                    {
                        if (y - 1 > 0)
                        {
                            neighbors[0, 0] = currentGrid[x - 1, y - 1];
                        }

                        if (y + 1 < GridSizeY)
                        {
                            neighbors[0, 2] = currentGrid[x - 1, y + 1];
                        }

                        neighbors[0, 1] = currentGrid[x - 1, y];
                    }
                    else if (x - 1 < 0)
                    {
                        neighbors[0, 0] = Dead;
                        neighbors[0, 1] = Dead;
                        neighbors[0, 2] = Dead;
                    }

                    if (x + 1 < GridSizeX)
                    {
                        if (y - 1 > 0)
                        {
                            neighbors[2, 0] = currentGrid[x + 1, y - 1];
                        }

                        if (y + 1 < GridSizeY)
                        {
                            neighbors[2, 2] = currentGrid[x + 1, y + 1];
                        }

                        neighbors[2, 1] = currentGrid[x + 1, y];
                    }
                    else if (x + 1 > GridSizeX)
                    {
                        neighbors[2, 0] = Dead;
                        neighbors[2, 1] = Dead;
                        neighbors[2, 2] = Dead;
                    }

                    if (y - 1 > 0)
                    {
                        neighbors[1, 0] = currentGrid[x, y - 1];
                    }
                    else if (y - 1 < 0)
                    {
                        neighbors[1, 0] = Dead;
                    }

                    if (y + 1 < GridSizeY)
                    {
                        neighbors[1, 2] = currentGrid[x, y + 1];
                    }
                    else if (y + 1 > GridSizeY)
                    {
                        neighbors[1, 2] = Dead;
                    }

                    neighbors[1, 1] = currentGrid[x, y];

                    livingNeighbors = CountLiveCells(neighbors, 3, 3);

                    if (currentGrid[x, y] == Alive)
                    {
                        livingNeighbors--;
                        if (livingNeighbors == 2 || livingNeighbors == 3)
                        {
                            nextGrid[x, y] = Alive;
                        }
                        else
                        {
                            nextGrid[x, y] = Dead;
                        }
                    }
                    else if (currentGrid[x, y] == Dead && livingNeighbors == 3)
                    {
                        nextGrid[x, y] = Alive;
                    }
                }
            }
        }

        static void GenerateRPentomino(int[,] grid)
        {
            int centerX = GridSizeX / 2;
            int centerY = GridSizeY / 2;

            if (GridSizeX % 2 == 1)
            {
                centerX++;
            }
            if (GridSizeY % 2 == 1)
            {
                centerY++;
            }

            grid[centerX, centerY - 1] = Alive;
            grid[centerX + 1, centerY - 1] = Alive;
            grid[centerX - 1, centerY] = Alive;
            grid[centerX, centerY] = Alive;
            grid[centerX, centerY + 1] = Alive;
        }
    }
}