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
            var keepalive = args.Length > 0 && args[0].Equals("-k");

            SimpleServer server = new SimpleServer(keepalive);
			server.Start();

			Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

			server.Stop();
		}
	}
}
