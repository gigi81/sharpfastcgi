using System;

namespace Grillisoft.FastCgi.Loggers.Log4Net
{
	[Serializable]
    public sealed class LoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type)
        {
            return new Logger(log4net.LogManager.GetLogger(type));
        }

        public ILogger Create(string name)
        {
            return new Logger(log4net.LogManager.GetLogger("fastcgi", name));
        }
    }
}
