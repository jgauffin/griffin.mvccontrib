using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib
{
	public class LogProvider
	{
		private static LogProvider _provider;
		private static bool _useDebug;

		public virtual ILogger GetLogger<T>() where T : class
		{
			if (!_useDebug)
				return NullLogger.Instance;

			return new DebugWindowLogger(typeof(T));
		}

		public static void UseDebugWindow()
		{
			_provider = new LogProvider();
			_useDebug = true;
		}

		public static LogProvider Current
		{
			set { _provider = value; }
			get { return _provider ?? (_provider = new LogProvider()); }
		}
	}
	public class NullLogger : ILogger
	{
		public static NullLogger Instance = new NullLogger();


		public void Debug(string message)
		{
			
		}

		public void Debug(string message, Exception exception)
		{
		}

		public void Warning(string message)
		{
		}

		public void Warning(string message, Exception exception)
		{
		}
	}

	public class DebugWindowLogger : ILogger
	{
		private readonly Type _type;

		public DebugWindowLogger(Type type)
		{
			_type = type;
		}

		public void Debug(string message)
		{
			Write(message);
		}

		public void Debug(string message, Exception exception)
		{
			Write(message + ": " + exception);
		}

		public void Warning(string message)
		{
			Write(message);
		}

		public void Warning(string message, Exception exception)
		{
			Write(message + ": " + exception);
		}

		private void Write(string message)
		{
			System.Diagnostics.Debug.WriteLine("");
			System.Diagnostics.Debug.WriteLine(_type.Name.PadRight(30, ' ') +  message);
		}
	}
	public interface ILogger
	{
		void Debug(string message);
		void Debug(string message, Exception exception);
		void Warning(string message);
		void Warning(string message, Exception exception);
	}
}
