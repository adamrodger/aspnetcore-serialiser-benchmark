using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
    public class PostRequest
    {
        private IDisposable server;
        private HttpClient client;
        private MyEventsListerViewModel data;

        private static readonly MediaTypeHeaderValue ContentTypeMsgPack = MediaTypeHeaderValue.Parse("application/x-msgpack");
        private static readonly MediaTypeHeaderValue ContentTypeProtobuf = MediaTypeHeaderValue.Parse("application/x-protobuf");

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
            this.data = DataGenerator.Generate<MyEventsListerViewModel>();
        }

        [GlobalCleanup]
        public void Stop()
        {
            this.server.Dispose();
        }

        [Benchmark(Baseline = true)]
        public async Task<HttpResponseMessage> Json()
        {
            string body = JsonConvert.SerializeObject(this.data);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await this.CallApiAsync(content);
            return response;
        }

        [Benchmark]
        public async Task<HttpResponseMessage> MessagePack()
        {
            byte[] body = MessagePackSerializer.Serialize(this.data);
            var content = new ByteArrayContent(body);
            content.Headers.ContentType = ContentTypeMsgPack;

            var response = await this.CallApiAsync(content);
            return response;
        }

        [Benchmark]
        public async Task<HttpResponseMessage> Protobuf()
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, this.data);
            stream.Position = 0;
            var content = new StreamContent(stream);
            content.Headers.ContentType = ContentTypeProtobuf;

            var response = await this.CallApiAsync(content);
            return response;
        }

        private async Task<HttpResponseMessage> CallApiAsync(HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/values") { Content = content };

            var response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
