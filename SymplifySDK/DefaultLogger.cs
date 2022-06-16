using System.Diagnostics;

namespace SymplifySDK
{
    /// <summary>
    /// A simple debuging ILogger.
    /// </summary>
    public class DefaultLogger : ILogger
    {
        /// <summary>
        /// Log the given message at the given log level.
        /// </summary>
        public void Log(LogLevel level, string message)
        {
            string line = $"[SSTSDK {level}] : {message}";
            Debug.WriteLine(line);
        }
    }
}
