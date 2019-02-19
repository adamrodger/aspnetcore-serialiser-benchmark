using System;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;

namespace AspNetCore.Benchmarks.Tests
{
    /// <summary>
    /// Export to InfluxDb line protocol format file
    /// </summary>
    /// <remarks>See https://docs.influxdata.com/influxdb/latest/write_protocols/line_protocol_reference/ </remarks>
    public class InfluxDbLineProtocolExporter : ExporterBase
    {
        protected override string FileNameSuffix => "-influxdb";
        protected override string FileExtension => "txt";

        /// <summary>
        /// Export data to a file
        /// </summary>
        /// <param name="summary">Benchmark summary data</param>
        /// <param name="logger">File writer</param>
        /// <remarks>InfluxDB requires LF line endings, not CRLF</remarks>
        public override void ExportToLog(Summary summary, ILogger logger)
        {
            long timestamp = (DateTimeOffset.UtcNow - DateTimeOffset.UnixEpoch).Ticks * 100; // 100ns per tick

            logger.Write("# DML\n# CONTEXT-DATABASE: benchmarks\n# CONTEXT-RETENTION-POLICY: \n\n");

            foreach (BenchmarkReport report in summary.Reports)
            {
                const string measurement = "benchmarks";
                string tags = $"type={report.BenchmarkCase.Descriptor.Type.Name},method={report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo}";

                Statistics stats = report.ResultStatistics;
                string fields = string.Join(',',
                                            $"mean={stats.Mean}",
                                            $"median={stats.Median}",
                                            $"min={stats.Min}",
                                            $"max={stats.Max}",
                                            $"std-dev={stats.StandardDeviation}",
                                            $"std-error={stats.StandardError}",
                                            $"variance={stats.Variance}",
                                            $"q1={stats.Q1}",
                                            $"q3={stats.Q3}",
                                            $"p95={stats.Percentiles.P95}",
                                            $"confidence-lower={stats.ConfidenceInterval.Lower}",
                                            $"confidence-upper={stats.ConfidenceInterval.Upper}",
                                            $"confidence-margin={stats.ConfidenceInterval.Margin}",
                                            $"kurtosis={stats.Kurtosis}",
                                            $"skewness={stats.Skewness}");

                logger.Write($"{measurement},{tags} {fields} {timestamp}\n");
            }
        }
    }
}
