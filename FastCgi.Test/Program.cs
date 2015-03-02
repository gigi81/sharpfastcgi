using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Hosting;
using Grillisoft.FastCgi.Tcp;
using Grillisoft.FastCgi.AspNet;
using System.Diagnostics;

namespace Grillisoft.FastCgi.Test
{
	class Program
	{
		static int Main(string[] args)
		{
            //Debugger.Launch();

            //var values = Environment.GetEnvironmentVariables();
            //foreach (var key in values.Keys)
            //{
            //    Debug.WriteLine("{0}: {1}", key, values[key]);
            //}

            //RunNamedPipeServer();
            RunTcpServer();
            return 0;
		}

        private static void RunTcpServer()
        {
            SimpleTcpServer server = new SimpleTcpServer();
            server.Start();

            Console.WriteLine("Press any key to stop the fastcgi server");
            Console.ReadKey();

            server.Stop();
        }

        private static void RunNamedPipeServer()
        {
            var path = Environment.GetEnvironmentVariable("_FCGI_X_PIPE_");
            SimpleNamedPipeServer server = new SimpleNamedPipeServer(path);
            server.Run();
        }
	}
}
