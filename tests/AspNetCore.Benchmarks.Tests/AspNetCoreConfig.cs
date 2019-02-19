using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Validators;

namespace AspNetCore.Benchmarks.Tests
{
    /// <summary>
    /// Config required to run ASP.Net Core in-memory hosted (TestServer) benchmarks
    /// </summary>
    public class AspNetCoreConfig : IConfig
    {
        public IEnumerable<IColumnProvider> GetColumnProviders() => DefaultColumnProviders.Instance;

        public IEnumerable<IExporter> GetExporters()
        {
            yield return new InfluxDbLineProtocolExporter();
            yield return CsvExporter.Default;
            yield return MarkdownExporter.GitHub;
        }

        public IEnumerable<ILogger> GetLoggers()
        {
            yield return ConsoleLogger.Default;
        }

        public IEnumerable<IDiagnoser> GetDiagnosers() => Array.Empty<IDiagnoser>();

        public IEnumerable<IAnalyser> GetAnalysers()
        {
            yield return EnvironmentAnalyser.Default;
            yield return OutliersAnalyser.Default;
            yield return MinIterationTimeAnalyser.Default;
            yield return MultimodalDistributionAnalyzer.Default;
            yield return RuntimeErrorAnalyser.Default;
            yield return ZeroMeasurementAnalyser.Default;
        }

        public IEnumerable<Job> GetJobs()
        {
            yield return Job.InProcess;
        }

        public IEnumerable<IValidator> GetValidators()
        {
            yield return BaselineValidator.FailOnError;
            yield return SetupCleanupValidator.FailOnError;
#if DEBUG
            yield return JitOptimizationsValidator.DontFailOnError;
#else
            yield return JitOptimizationsValidator.FailOnError;
#endif
            yield return RunModeValidator.FailOnError;
            yield return GenericBenchmarksValidator.DontFailOnError;
            yield return DeferredExecutionValidator.FailOnError;
        }

        public IEnumerable<HardwareCounter> GetHardwareCounters() => Array.Empty<HardwareCounter>();

        public IEnumerable<IFilter> GetFilters() => Array.Empty<IFilter>();

        public IOrderer GetOrderer() => null;

        public ISummaryStyle GetSummaryStyle() => SummaryStyle.Default;

        public IEnumerable<BenchmarkLogicalGroupRule> GetLogicalGroupRules() => Array.Empty<BenchmarkLogicalGroupRule>();

        public ConfigUnionRule UnionRule => ConfigUnionRule.AlwaysUseLocal;

        /// <summary>
        /// determines if all auto-generated files should be kept or removed after running the benchmarks
        /// </summary>
        public bool KeepBenchmarkFiles => false;

        /// <summary>
        /// determines if all benchmarks results should be joined into a single summary or not
        /// </summary>
        public bool SummaryPerType => false;

        /// <summary>the default value is "./BenchmarkDotNet.Artifacts"</summary>
        public string ArtifactsPath => Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts");

        /// <summary>the default value is ASCII</summary>
        public Encoding Encoding => Encoding.Default;

        public bool StopOnFirstError => true;
    }
}
