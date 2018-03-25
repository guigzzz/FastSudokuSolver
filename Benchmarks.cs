using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

static class Benchmarks
{
    public static void JSolveBenchmark(int averages)
    {
        //http://attractivechaos.github.io/plb/
        var watch = new Stopwatch();

        List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("data/worstcase.txt");
        
        SudokuSolver solver = new SudokuSolver();
        long times = 0;
        for(int a = 0; a < averages; a++)
        {
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
            times += watch.ElapsedTicks;
        }
        
        double time = times / (double)Stopwatch.Frequency;
        time /= averages;

        Console.WriteLine(String.Format(
            "Average Time over {2} Jsolve benchmarks: {0:#.####}s\nAverage time per grid: {1:#.####}s", 
            time, time / (50 * sudokus.Count), averages
        ));
    }
    public static void OneMillionBenchmark()
    {
        // https://www.kaggle.com/bryanpark/sudoku
        List<Tuple<int[], int[]>> sudokus = SudokuCSV.readFromCSV("data/sudoku.csv", 1000000);
        SudokuSolver solver = new SudokuSolver();

        var watch = new Stopwatch();
        watch.Start();
        
        foreach(var s in sudokus)
        {
            Sudoku solved = solver.solve(s.Item1);
            if(!solved.grid.SequenceEqual(s.Item2))
            {
                throw new Exception(String.Format(
                    "Failed for:\n {0}\n Got:\n{1}\nExpected:\n{2}\n", 
                    SudokuUtils.gridToString(s.Item1),
                    SudokuUtils.gridToString(solved.grid),
                    SudokuUtils.gridToString(s.Item2)
                    ));
            }
        }
        watch.Stop();
        
        double time = watch.ElapsedTicks / (double)Stopwatch.Frequency;

        Console.WriteLine(String.Format(
            "Time for One Million Sudokus Benchmark: {0:#.####}s\nAverage time per grid: {1}s", 
            time, time / sudokus.Count
        ));
    }

    public static void sudoku1465Benchmark()
    {
        List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("data/top1465.txt");
        
        SudokuSolver solver = new SudokuSolver();

        var watch = new Stopwatch();
        watch.Start();
        for(int i = 0; i < 64; i++)
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
            "Time for 1465*64 benchmark: {0:#.####}s\nAverage time per grid: {1:#.####}s", 
            time, time / (50 * sudokus.Count)
        ));
    }
}