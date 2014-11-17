using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using FastCgi.AspNet;
using System.Configuration;

namespace FastCgi.Server
{
    class Program
    {
        public const int DefaultPort = 9000;
        public const string DefaultVirtualPath = "/";
        public const string DefaultPhysicalPath = "Root";

        private static string VirtualPath
        {
            get { return ConfigurationManager.AppSettings["VirtualPath"] ?? DefaultVirtualPath; }
        }

        private static string PhysicalPath
        {
            get
            {
                return Path.GetFullPath(ConfigurationManager.AppSettings["PhysicalPath"] ?? DefaultPhysicalPath);
            }
        }

        private static int Port
        {
            get
            {
                int ret;
                if (Int32.TryParse(ConfigurationManager.AppSettings["Port"], out ret))
                    return ret;

                return DefaultPort;
            }
        }

        private static bool KeepConnection
        {
            get
            {
                bool ret;
                if (Boolean.TryParse(ConfigurationManager.AppSettings["KeepConnection"], out ret))
                    return ret;

                return false;
            }
        }

        static void Main(string[] args)
        {
            var server = FastCgiAspNetServer.CreateApplicationHost(Port, VirtualPath, PhysicalPath, KeepConnection);
            server.Start();

			Console.WriteLine ("Listening on port {0}", Port);
            Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

            server.Stop();
        }
    }
}
