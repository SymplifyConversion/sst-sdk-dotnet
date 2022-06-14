using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace SymplifySDK.Tests
{
    public class Loggingtest
    {

        [Fact]
        public void testErrorLog()
        {
            LogSpy logSpy = new();
            ClientConfig cfg = new("4711", "https://localhost:10000");
            SymplifyClient sdk = new(cfg, new HttpClient(), logSpy);
            List<string> projects = sdk.ListProjects();

            Assert.Empty(projects);

            Assert.NotEmpty(logSpy.SeenMessages);
            Assert.Equal("listProjects called before config is available", logSpy.SeenMessages[0].Item2);
            Assert.Equal("ERROR", logSpy.SeenMessages[0].Item1);
        }
    }


    public class LogSpy : ILogger
    {
        public List<(string, string)> SeenMessages { get; set; }

        public LogSpy()
        {
            SeenMessages = new List<(string, string)>();
        }

        public void Log(LogLevel level, string message)
        {
            SeenMessages.Add((level.ToString(), message));
        }
    }
}
