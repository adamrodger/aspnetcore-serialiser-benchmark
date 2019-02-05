using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Benchmark;
using BenchmarkDotNet.Attributes;
using MessagePack;
using MicroBenchmarks.Serializers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ProtoBuf;

namespace AspNetCore.Benchmarks.Tests
{
    public class SerialiserBenchmark
    {
        private IDisposable server;
        private HttpClient client;

        [GlobalSetup]
        public void Start()
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(config =>
            {
                config.ConfigureAppConfiguration((context, builder) =>
                {
                    context.HostingEnvironment.ContentRootPath = @"D:\git\aspnetcore-serialiser-benchmark\src\AspNetCore.Benchmarks";
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
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("/api/values"),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            var response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MyEventsListerViewModel>(body);
        }

        [Benchmark]
        public async Task<MyEventsListerViewModel> MessagePack()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("/api/values"),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/x-msgpack"));

            var response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            byte[] body = await response.Content.ReadAsByteArrayAsync();
            return MessagePackSerializer.Deserialize<MyEventsListerViewModel>(body);
        }

        [Benchmark]
        public async Task<MyEventsListerViewModel> Protobuf()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("/api/values"),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/x-protobuf"));

            var response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Stream body = await response.Content.ReadAsStreamAsync();
            return Serializer.Deserialize<MyEventsListerViewModel>(body);
        }
    }
}
