using System;
using System.Threading;
using Grillisoft.FastCgi.Servers;
using log4net;

namespace Grillisoft.FastCgi.Test
{
	class Program
	{
        static int Main(string[] args)
		{
            var logger = LoggerFactory.Create("Test");
            logger.Log(LogLevel.Info, "Starting fastcgi server");

            var server = CreateServer();
            server.Start();

            while(true)
            {
                Thread.Sleep(1000);
            }

            return 0;
		}

        private static IFastCgiServer CreateServer()
        {
            return new IisServer(new SimpleChannelFactory(LoggerFactory), LoggerFactory);
        }

        private static ILoggerFactory _loggerFactory;

        private static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory != null)
                    return _loggerFactory;

                return _loggerFactory = new Grillisoft.FastCgi.Loggers.Log4Net.LoggerFactory();
            }
        }
	}
}
