using BenchmarkDotNet.Running;

internal class Program
{
    private static void Main(string[] args)
    {
        // BenchmarkRunner.Run<WorstcaseBench>();
        // BenchmarkRunner.Run<Top1465Bench>();
        BenchmarkRunner.Run<Top10Bench>();

        // var bench = new worstcaseBench();

        // for (var i = 0; i < 5; i++)
        // {
        //     Console.WriteLine(i);
        // bench.solve();
        // }
    }
}