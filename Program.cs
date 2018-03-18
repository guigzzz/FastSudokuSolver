using System;
using System.Diagnostics;

namespace SudokuSolverC_
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            
            Sudoku sudoku = new Sudoku();
            sudoku.loadFromFile(file_name: "sudoku.txt");
            sudoku.print();

            SudokuSolver solver = new SudokuSolver();

            watch.Start();
            sudoku = solver.solve(sudoku);
            watch.Stop();
            double time = watch.ElapsedTicks / (double)Stopwatch.Frequency;

            Console.WriteLine(String.Format(
                "Time: {0:#.####}s", time
            ));

            sudoku.print();

            if(SolutionChecker.isValidSudokuSolution(sudoku))
                Console.WriteLine("Solution Valid!");
            else
                Console.WriteLine("Solution Invalid...");
        }
    }
}
