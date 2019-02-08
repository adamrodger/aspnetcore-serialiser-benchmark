using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Benchmark;
using BenchmarkDotNet.Attributes;
using MessagePack;
using MicroBenchmarks.Serializers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProtoBuf;

namespace AspNetCore.Benchmarks.Tests
{
    [Config(typeof(AspNetCoreConfig))]
    public class GetRequest
    {
        private IDisposable server;
        private HttpClient client;

        private static readonly MediaTypeWithQualityHeaderValue AcceptJson = MediaTypeWithQualityHeaderValue.Parse("application/json");
        private static readonly MediaTypeWithQualityHeaderValue AcceptMsgPack = MediaTypeWithQualityHeaderValue.Parse("application/x-msgpack");
        private static readonly MediaTypeWithQualityHeaderValue AcceptProtobuf = MediaTypeWithQualityHeaderValue.Parse("application/x-protobuf");

        [GlobalSetup]
        public void Start()
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(configuration =>
            {
                configuration.ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // stop the output being incredibly verbose since we're doing thousands of requests
                });
            });
            this.server = factory;
            this.client = factory.CreateDefaultClient();
        }

        [GlobalCleanup]
        public void Stop()
        {
            this.server.Dispose();
        }

        [Benchmark(Baseline = true)]
        public async Task<MyEventsListerViewModel> Json()
        {
            var response = await this.CallApiAsync(AcceptJson);

            string body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MyEventsListerViewModel>(body);
        }

        [Benchmark]
        public async Task<MyEventsListerViewModel> MessagePack()
        {
            var response = await this.CallApiAsync(AcceptMsgPack);

            byte[] body = await response.Content.ReadAsByteArrayAsync();
            return MessagePackSerializer.Deserialize<MyEventsListerViewModel>(body);
        }

        [Benchmark]
        public async Task<MyEventsListerViewModel> Protobuf()
        {
            var response = await this.CallApiAsync(AcceptProtobuf);

            Stream body = await response.Content.ReadAsStreamAsync();
            return Serializer.Deserialize<MyEventsListerViewModel>(body);
        }

        private async Task<HttpResponseMessage> CallApiAsync(MediaTypeWithQualityHeaderValue acceptHeader)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/values");
            request.Headers.Accept.Add(acceptHeader);

            var response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
