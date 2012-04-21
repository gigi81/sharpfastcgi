using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;
using FastCgi.Tcp;
using FastCgi.AspNet;

namespace FastCgi.Test
{
    class Program
    {
        static void Main(string[] args)
        {
			string path = Path.Combine(Directory.GetCurrentDirectory(), "Root");
			SimpleServer server = (SimpleServer)ApplicationHost.CreateApplicationHost(typeof(SimpleServer), "/simple", path);
            server.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
