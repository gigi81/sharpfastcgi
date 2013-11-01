using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using FastCgi.AspNet;

namespace FastCgi.Server
{
    class Program
    {
        public const int DefaultPort = 9000;

        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Root");
            var server = FastCgiAspNetServer.CreateApplicationHost(DefaultPort, "/", path);
            server.Start();

            Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

            server.Stop();
        }
    }
}
