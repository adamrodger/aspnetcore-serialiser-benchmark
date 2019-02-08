using BenchmarkDotNet.Running;

namespace AspNetCore.Benchmarks.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
        }
    }
}
