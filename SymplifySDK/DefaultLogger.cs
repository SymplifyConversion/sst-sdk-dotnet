using System.Diagnostics;

namespace SymplifySDK
{
    public class DefaultLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            string line = $"[SSTSDK {level}] : {message}";
            Debug.WriteLine(line);
        }
    }
}
