using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Running;


class Program
{
    static void Main(string[] args)
    {
        // Benchmarks.JSolveBenchmark(1);
        // Benchmarks.OneMillionBenchmark();
        // Benchmarks.sudoku1465Benchmark();

        var summary = BenchmarkRunner.Run<top10Bench>();
    }
}
