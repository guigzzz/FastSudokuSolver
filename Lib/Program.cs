using BenchmarkDotNet.Running;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<worstcaseBench>();
        // BenchmarkRunner.Run<top1465Bench>();
        // BenchmarkRunner.Run<top10Bench>();
    }
}