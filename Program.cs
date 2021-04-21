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

        static int GridSizeX = 20;
        static int GridSizeY = 10;

        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("=== Starting Program ===");

            int gridCount = 0;
            int[,] grid = new int[GridSizeX, GridSizeY];

            bool done = false;
            while (!done)
            {
                // Randomly Fill the Grid
                FillGridRandomly(grid, 20);

                // Display the grid (and log its statistics)
                WriteLine($"Grid #{gridCount}");
                PrintGrid(grid);
                logger.Debug($"Grid #{gridCount}  aliveCount: {CountLiveCells(grid)}");

                Thread.Sleep(500);

                // Check to see if the user pressed a key
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    logger.Debug($"{key} pressed...");
                    if (key == ConsoleKey.Q)
                        done = true;
                }

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

        static int CountLiveCells(int[,] grid)
        {
            int count = 0;
            for (int x = 0; x < GridSizeX; x++)
                for (int y = 0; y < GridSizeY; y++)
                    if (grid[x, y] == Alive)
                        count++;
            return count;
        }
    }
}