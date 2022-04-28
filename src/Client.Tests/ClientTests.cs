using System.Collections.Generic;
using Allocation.Config;
using Xunit;

namespace Client.Test
{
    public class ClientTests
    {
        [Fact]
        public void TestCreateInvalidCDNBaseURL()
        {
            var cfg = new ClientConfig("4711");
        }
    }
}
