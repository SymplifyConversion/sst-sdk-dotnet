using Xunit;
using SymplifySDK.Allocation.Config;

namespace SymplifySDK.Tests
{
    public class ClientConfigTest
    {
        [Fact]
        public void TestHashDefaults()
        {
            ClientConfig cfg = new ClientConfig("goober");
            Assert.Equal("goober", cfg.WebsiteID);
            Assert.Equal("https://cdn-sitegainer.com", cfg.CdnBaseURL);
        }
    }
}
