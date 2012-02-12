using System;

namespace Griffin.MvcContrib.Logging
{
    /// <summary>
    /// Logs to nothing
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// Gets logger instance.
        /// </summary>
        public static NullLogger Instance = new NullLogger();

        private NullLogger()
        {
        }

        #region ILogger Members

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Message to write</param>
        public void Debug(string message)
        {
        }

        /// <summary>
        /// Write a debug message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        public void Debug(string message, Exception exception)
        {
        }

        /// <summary>
        /// Warning (something unexpected but the framework can continue as expected)
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
        }

        /// <summary>
        /// Write a warning message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        public void Warning(string message, Exception exception)
        {
        }

        #endregion
    }
}