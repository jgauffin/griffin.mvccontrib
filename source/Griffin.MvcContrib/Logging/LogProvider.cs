namespace Griffin.MvcContrib.Logging
{
    /// <summary>
    /// Used to enable logging in the framework
    /// </summary>
    public class LogProvider
    {
        private static LogProvider _provider;
        private static bool _useDebug;

        /// <summary>
        /// Gets current log provider
        /// </summary>
        public static LogProvider Current
        {
            set { _provider = value; }
            get { return _provider ?? (_provider = new LogProvider()); }
        }

        /// <summary>
        /// Get a logger for the specified type
        /// </summary>
        /// <typeparam name="T">Type of class that want's to log information</typeparam>
        /// <returns>A logger</returns>
        public virtual ILogger GetLogger<T>() where T : class
        {
            if (!_useDebug)
                return NullLogger.Instance;

            return new DebugWindowLogger(typeof (T));
        }

        /// <summary>
        /// Log to the VStudio debug window
        /// </summary>
        public static void UseDebugWindow()
        {
            _provider = new LogProvider();
            _useDebug = true;
        }
    }
}