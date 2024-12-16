using System;
using System.Net.Http;

using RichardSzalay.MockHttp;
using Xunit;

namespace Symplify.Conversion.SDK.Tests
{
    public class ClientTests
    {
        public HttpClient fakeHttpClient(string responseBody = "{}", string contentType = "application/json")
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*").Respond(contentType, responseBody);

            return mockHttp.ToHttpClient();
        }

        [Fact]
        public void CreateInvalidCDNBaseURL()
        {
            ClientConfig cfg = new ClientConfig("4711", "cdn.example.com");
            Assert.Throws<UriFormatException>(() => new SymplifyClient(cfg, fakeHttpClient(), new DefaultLogger()));
        }

        [Fact]
        public void TestGetConfigURLDefaultCDN()
        {
            //5620148 test website id
            var client = new SymplifyClient("4711", fakeHttpClient());
            Assert.Equal("https://cdn-sitegainer.com/4711/sstConfig.json", client.GetConfigURL());
        }

        [Fact]
        public void TestGetConfigURLOverrideCDN()
        {
            ClientConfig cfg = new ClientConfig("1337", "https://cdn.example.com");
            SymplifyClient client = new SymplifyClient(cfg, fakeHttpClient(), new DefaultLogger());

            Assert.Equal("https://cdn.example.com/1337/sstConfig.json", client.GetConfigURL());
        }

        [Fact]
        public void TestGetConfigURLLocalhostCDN()
        {
            ClientConfig cfg = new ClientConfig("42", "http://localhost:9000");
            SymplifyClient client = new SymplifyClient(cfg, fakeHttpClient(), new DefaultLogger());

            Assert.Equal("http://localhost:9000/42/sstConfig.json", client.GetConfigURL());
        }

        [Fact]
        async public void TestLoadConfig()
        {
            string testConfig = @"{
                ""updated"": 1648466732,
                ""projects"": [
                    {
                        ""id"": 4711,
                        ""name"": ""discount"",
                        ""variations"": [
                            { ""id"":   42, ""name"": ""original"", ""weight"": 10, ""distribution"": 10 },
                            { ""id"": 1337, ""name"": ""huge""    , ""weight"":  2, ""distribution"": 2 },
                            { ""id"": 9999, ""name"": ""small""   , ""weight"":  1, ""distribution"": 1 }
                        ]
                    }
            ]}";

            ClientConfig cfg = new ClientConfig("42", "http://localhost:9000");
            SymplifyClient client = new SymplifyClient(cfg, fakeHttpClient(testConfig), new DefaultLogger());

            Assert.Empty(client.ListProjects());

            await client.LoadConfig();

            Assert.Single(client.ListProjects());
        }
    }
}
