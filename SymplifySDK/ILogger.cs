namespace SymplifySDK
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
    }

    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }
}
