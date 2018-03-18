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
            long time = watch.ElapsedMilliseconds;

            Console.WriteLine(String.Format(
                "Time: {0}", time
            ));

            sudoku.print();
        }
    }
}
