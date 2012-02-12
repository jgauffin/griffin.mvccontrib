using System;

namespace Griffin.MvcContrib.Logging
{
    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Message to write</param>
        void Debug(string message);

        /// <summary>
        /// Write a debug message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Warning (something unexpected but the framework can continue as expected)
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);

        /// <summary>
        /// Write a warning message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        void Warning(string message, Exception exception);
    }
}