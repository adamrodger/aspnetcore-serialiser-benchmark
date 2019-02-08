using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;

namespace AspNetCore.Benchmarks.Tests
{
    /// <summary>
    /// Config required to run ASP.Net Core in-memory hosted (TestServer) benchmarks
    /// </summary>
    public class AspNetCoreConfig : ManualConfig
    {
        /// <summary>
        /// Initialises a new instance of the see <see cref="AspNetCoreConfig"/> class.
        /// </summary>
        public AspNetCoreConfig()
        {
            this.Add(Job.InProcessDontLogOutput);

            this.Add(JsonExporter.Default);

#if DEBUG
            // allow running as Debug inside VS
            this.Add(JitOptimizationsValidator.DontFailOnError);
#endif
        }
    }
}
