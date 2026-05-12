using BenchmarkDotNet.Running;
using Kinetica.Benchmarks;

// Check for command-line arguments
if (args.Length > 0)
{
    var command = args[0].ToLowerInvariant();
    if (command == "avro-standalone" || command == "--avro-standalone")
    {
        AvroDecodeBenchmarkRunner.Run();
        return;
    }
    else if (command == "avro" || command == "--avro")
    {
        var summary = BenchmarkRunner.Run<AvroDecodeBenchmarks>();
        PrintSummary(summary);
        return;
    }
}

// Default: Run BulkInserter benchmarks
{
    var summary = BenchmarkRunner.Run<BulkInserterBenchmarks>();
    PrintSummary(summary);
}

static void PrintSummary(BenchmarkDotNet.Reports.Summary summary)
{
    Console.WriteLine("\nBenchmark Summary:");
    Console.WriteLine("==================");
    Console.WriteLine($"Total benchmarks run: {summary.BenchmarksCases.Length}");
    Console.WriteLine($"Total time: {summary.TotalTime}");

    // Print a simple summary
    foreach (var report in summary.Reports)
    {
        if (report.Success)
        {
            var mean = report.ResultStatistics?.Mean ?? 0;
            var ops = mean > 0 ? 1_000_000_000 / mean : 0; // ops per second
            Console.WriteLine($"{report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo}: {mean / 1_000_000:F2}ms mean, ~{ops:F0} ops/s");
        }
        else
        {
            Console.WriteLine($"{report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo}: FAILED");
        }
    }
}
