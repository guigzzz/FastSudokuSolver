using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Benchmarks.top10sudokus(1000);
        // Benchmarks.JSolveBenchmark(1);
        // Benchmarks.OneMillionBenchmark();
        // Benchmarks.sudoku1465Benchmark();
    }
}
