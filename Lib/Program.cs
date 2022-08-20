using BenchmarkDotNet.Running;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<worstcaseBench>();
        // BenchmarkRunner.Run<top1465Bench>();
        // BenchmarkRunner.Run<top10Bench>();

        // var bench = new worstcaseBench();

        // for (var i = 0; i < 5; i++)
        // {
        //     Console.WriteLine(i);
        // bench.solve();
        // }
    }
}