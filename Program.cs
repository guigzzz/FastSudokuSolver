using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SudokuSolverC_
{
    class Program
    {
        static void Main(string[] args)
        {
            // var watch = new Stopwatch();

            // List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("top1465.txt");
            
            // SudokuSolver solver = new SudokuSolver();

            // watch.Start();
            // foreach(var s in sudokus)
            // {
            //     Sudoku solved = solver.solve(s);
            //     if(!SudokuUtils.isValidSudokuSolution(solved))
            //         throw new Exception(String.Format(
            //             "Failed for:\n {0}\n Got:\n{1}\n", 
            //             SudokuUtils.gridToString(s),
            //             SudokuUtils.gridToString(solved.grid)
            //             ));

            // }
            // watch.Stop();
            // double time = watch.ElapsedTicks / (double)Stopwatch.Frequency;

            // Console.WriteLine(String.Format(
            //     "Time: {0:#.####}s", time
            // ));
            JSolveBenchmark();
        }


        static void JSolveBenchmark()
        {
            //http://attractivechaos.github.io/plb/
            var watch = new Stopwatch();

            List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("worstcase.txt");
            
            SudokuSolver solver = new SudokuSolver();

            watch.Start();
            for(int i = 0; i < 50; i++)
            {
                foreach(var s in sudokus)
                {
                    Sudoku solved = solver.solve(s);
                    if(!SudokuUtils.isValidSudokuSolution(solved))
                        throw new Exception(String.Format(
                            "Failed for:\n {0}\n Got:\n{1}\n", 
                            SudokuUtils.gridToString(s),
                            SudokuUtils.gridToString(solved.grid)
                            ));
                }
            }
            
            watch.Stop();
            double time = watch.ElapsedTicks / (double)Stopwatch.Frequency;

            Console.WriteLine(String.Format(
                "Time for Jsolve benchmark: {0:#.####}s", time
            ));
        }

        // static void Main(string[] args)
        // {
        //     List<Tuple<int[], int[]>> sudokus = SudokuCSV.readFromCSV("sudoku.csv", 1000000);
        //     SudokuSolver solver = new SudokuSolver();

        //     var watch = new Stopwatch();
        //     watch.Start();
            
        //     foreach(var s in sudokus)
        //     {
        //         Sudoku solved = solver.solve(s.Item1);
        //         if(!solved.grid.SequenceEqual(s.Item2))
        //         {
        //             throw new Exception(String.Format(
        //                 "Failed for:\n {0}\n Got:\n{1}\nExpected:\n{2}\n", 
        //                 SudokuUtils.gridToString(s.Item1),
        //                 SudokuUtils.gridToString(solved.grid),
        //                 SudokuUtils.gridToString(s.Item2)
        //                 ));
        //         }
        //     }
        //     watch.Stop();

        //     Console.WriteLine(String.Format(
        //         "> {0} Sudokus passed", sudokus.Count
        //         ));

            
        //     double time = watch.ElapsedTicks / (double)Stopwatch.Frequency;

        //     Console.WriteLine(String.Format(
        //         "Total time: {0:##.##}s\nAverage time per grid: {1}s", 
        //         time, time / sudokus.Count
        //     ));
        // }
    }
}
