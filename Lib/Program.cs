using BenchmarkDotNet.Running;

internal class Program
{
    private static void Main(string[] _)
    {
        // BenchmarkRunner.Run<WorstcaseBench>();
        // BenchmarkRunner.Run<Top1465Bench>();
        BenchmarkRunner.Run<Top10Bench>();

        // var bench = new Top10Bench();

        // while (true)
        // {
        //     bench.Solve();
        // }
    }
}