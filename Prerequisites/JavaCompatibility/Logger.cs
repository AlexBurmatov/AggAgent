using System;

namespace JavaCompatibility
{
	public class Logger
	{
		private readonly String category;
		private static readonly log4net.ILog log =
			log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Logger(String category)
		{
			this.category = category;
		}

		public static Logger getLogger(string category)
		{
			return new Logger(category);
		}

		public void debug(string s)
		{
			log.Debug(category + ": " + s);
		}

		public void info(string s)
		{
			log.Info(category + ": " + s);
		}

		public void warn(string s)
		{
			log.Warn(category + ": " + s);
		}

		public void error(string s)
		{
			log.Error(category + ": " + s);
		}

		public void error(string s, Exception exception)
		{
			log.Error(category + ": " + s + " " + exception);
		}

		public void debug(string s, Exception exception)
		{
			log.Debug(category + ": " + s + " " + exception);
		}

		public void warn(string s, Exception exception)
		{
			log.Warn(category + ": " + s + " " + exception);
		}

		public void info(string s, Exception exception)
		{
			log.Info(category + ": " + s + " " + exception);
		}

		public bool isDebugEnabled()
		{
			return false;
		}
	}
}