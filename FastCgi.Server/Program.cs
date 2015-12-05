using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Grillisoft.FastCgi.AspNet;
using System.Configuration;
using Grillisoft.FastCgi.Servers;
using Topshelf;
using log4net;

namespace Grillisoft.FastCgi.Server
{
    class Program
    {
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Logger.Info("Starting FastCgi Server");

            HostFactory.Run(x =>
            {
                x.Service<FastCgiAspNetServer>(s =>
                {
                    s.ConstructUsing(name => CreateServer());
                    s.WhenStarted(tc => {
                        tc.Start();
                        Logger.InfoFormat("Listening on {0}:{1}", Config.Address, Config.Port);
                    });
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetDescription("Asp.NET FastCgi Service");
                x.SetDisplayName("Asp.NET FastCgi Service");
                x.SetServiceName("aspnet-fastcgi");
                x.UseLinuxIfAvailable();
                x.UseLog4Net();
            });
        }

        static FastCgiAspNetServer CreateServer()
        {
            var loggerFactory = new Loggers.Log4Net.LoggerFactory();
            return FastCgiAspNetServer.CreateApplicationHost(Config.Address, Config.Port, Config.VirtualPath, Config.PhysicalPath, loggerFactory);
        }
    }
}
