using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;
using Grillisoft.FastCgi.Servers;
using Grillisoft.FastCgi.AspNet;
using System.Diagnostics;
using System.Threading;

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
