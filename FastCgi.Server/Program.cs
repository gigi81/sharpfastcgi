using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Grillisoft.FastCgi.AspNet;
using System.Configuration;

namespace Grillisoft.FastCgi.Server
{
    class Program
    {
        static void Main(string[] args)
        {
			var server = FastCgiAspNetServer.CreateApplicationHost(Config.Port, Config.VirtualPath, Config.PhysicalPath);
            server.Start();

			Console.WriteLine ("Listening on port {0}", Config.Port);
            Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

            server.Stop();
        }
    }
}
