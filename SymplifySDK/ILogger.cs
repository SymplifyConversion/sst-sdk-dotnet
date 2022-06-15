namespace SymplifySDK
{
    /// <summary>
    /// LogLevel is used for filtering log messages.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Used when reporting debug info.
        /// </summary>
        DEBUG,

        /// <summary>
        /// Used when reporting informational messages.
        /// </summary>
        INFO,

        /// <summary>
        /// Used when reporting warning messages.
        /// </summary>
        WARN,

        /// <summary>
        /// Used when reporting errors.
        /// </summary>
        ERROR,
    }

    /// <summary>
    /// An interface used for collecting log messages from inside the SDK.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log the given message at the given log level.
        /// </summary>
        void Log(LogLevel level, string message);
    }
}
