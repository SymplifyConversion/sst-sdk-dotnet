using System;
using Allocation.Config;
using Xunit;

namespace Client.Test
{
    public class ClientTests
    {
        [Fact]
        public void CreateInvalidCDNBaseURL()
        {
            ClientConfig cfg = new ClientConfig("4711", "cdn.example.com");
            Assert.Throws<UriFormatException>(() => new Client(cfg));
        }

        [Fact]
        public async void TestGetConfigURLDefaultCDN()
        {
            //5620148 test website id
            var client = await Client.WithDefault("4711");
            Assert.Equal("https://cdn-sitegainer.com/4711/sstConfig.json", client.GetConfigURL());
        }

        [Fact]
        public void TestGetConfigURLOverrideCDN()
        {
            ClientConfig cfg = new ClientConfig("1337", "https://cdn.example.com");
            Client client = new Client(cfg);

            Assert.Equal("https://cdn.example.com/1337/sstConfig.json", client.GetConfigURL());
        }

        [Fact]
        public void TestGetConfigURLLocalhostCDN()
        {
            ClientConfig cfg = new ClientConfig("42", "http://localhost:9000");
            Client client = new Client(cfg);

            Assert.Equal("http://localhost:9000/42/sstConfig.json", client.GetConfigURL());
        }

    }
}
