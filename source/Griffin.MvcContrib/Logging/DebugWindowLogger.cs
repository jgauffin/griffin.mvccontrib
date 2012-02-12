using System;

namespace Griffin.MvcContrib.Logging
{
    /// <summary>
    /// Log to VStudio debug window
    /// </summary>
    public class DebugWindowLogger : ILogger
    {
        private readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugWindowLogger"/> class.
        /// </summary>
        /// <param name="type">The logging type.</param>
        public DebugWindowLogger(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            _type = type;
        }

        #region ILogger Members

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="message">Message to write</param>
        public void Debug(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Write a debug message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        public void Debug(string message, Exception exception)
        {
            Write(message + ": " + exception);
        }

        /// <summary>
        /// Warning (something unexpected but the framework can continue as expected)
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Write a warning message and recursive exception trace (write out all inner exceptions)
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="exception">Exception to log</param>
        public void Warning(string message, Exception exception)
        {
            Write(message + ": " + exception);
        }

        #endregion

        private void Write(string message)
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine(_type.Name.PadRight(30, ' ') + message);
        }
    }
}