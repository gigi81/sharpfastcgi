using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Grillisoft.FastCgi.AspNet;
using System.Configuration;
using Grillisoft.FastCgi.Servers;

namespace Grillisoft.FastCgi.Server
{
    class Program
    {
		private static ILoggerFactory loggerFactory;

        static void Main(string[] args)
        {
			loggerFactory = new Grillisoft.FastCgi.Loggers.Log4Net.LoggerFactory();

			var server = FastCgiAspNetServer.CreateApplicationHost(Config.Address, Config.Port, Config.VirtualPath, Config.PhysicalPath, loggerFactory);
            server.Start();

			Console.WriteLine ("Listening on port {0}", Config.Port);
            Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

            server.Stop();
        }
    }
}
