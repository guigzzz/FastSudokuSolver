using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

static class Benchmarks
{
    public static void JSolveBenchmark()
    {
        //http://attractivechaos.github.io/plb/
        var watch = new Stopwatch();

        List<int[]> sudokus = SudokuUtils.loadFromFileDotNotation("data/worstcase.txt");
        
        SudokuSolver solver = new SudokuSolver();
        watch.Start();
        for(int i = 0; i < 50; i++)
        {
            foreach(var s in sudokus)
            {
                Sudoku solved = solver.solve(s);
                if(!SudokuUtils.isValidSudokuSolution(solved, s))
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
            "Average Time for Jsolve benchmark: {0:#.####}s\nAverage time per grid: {1:#.####}s", 
            time, time / (50 * sudokus.Count)
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
                if(!SudokuUtils.isValidSudokuSolution(solved, s))
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


public class top10Bench
{
    List<int[]> sudokus = new List<string>(){
            "1....7.9..3..2...8..96..5....53..9...1..8...26....4...3......1..4......7..7...3..", //AI escargot
            ".......7..6..1...4..34..2..8....3.5...29..7...4..8...9.2..6...7...1..9..7....8.6.", //AI killer application
            "1..5..4....9.3.....7...8..5..1....3.8..6..5...9...7..8..4.2..1.2..8..6.......1..2", //AI lucky diamond
            ".8......1..7..4.2.6..3..7....2..9...1...6...8.3.4.......17..6...9...8..5.......4.", //AI worm hole
            "1..4..8...4..3...9..9..6.5..5.3..........16......7...2..4.1.9..7..8....4.2...4.8.", //AI labyrinth
            "..5..97...6.....2.1..8....6.1.7....4..7.6..3.6....32.......6.4..9..5.1..8..1....2", //AI circles
            "6.....2...9...1..5..8.3..4......2..15..6..9....7.9.....7...3..2...4..5....6.7..8.", //AI squadron
            "1......6....1....3..5..29....9..1...7...4..8..3.5....25..4....6..8.6..7..7...5...", //AI honeypot
            "....1...4.3.2.....6....8.9...7.6...59....5.8....8..4...4.9..1..7....2.4...5.3...7", //AI tweezers
            "4...6..7.......6...3...2..17....85...1.4......2.95..........7.5..91...3...3.4..8." //AI broken brick
        }.Select(i => SudokuUtils.parseLine(i)).ToList(); 

    SudokuSolver solver = new SudokuSolver();

    [Benchmark]
    public void solve()
    {
        foreach(var s in sudokus)
        {
            solver.solve(s);
        }
    }
}