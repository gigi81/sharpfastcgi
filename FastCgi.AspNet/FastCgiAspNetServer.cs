using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Threading;
using System.Web.Hosting;
using Grillisoft.FastCgi.Protocol;
using Grillisoft.FastCgi.Servers;

namespace Grillisoft.FastCgi.AspNet
{
    public class FastCgiAspNetServer : MarshalByRefObject
    {
		private IFastCgiServer _server;

        /// <summary>
        /// Create a fastcgi server
        /// </summary>
        /// <param name="port">Tcp/ip port where the server will be listening for fastcgi requests</param>
        /// <param name="virtualPath">Virtual path of your ASP.NET application</param>
        /// <param name="physicalPath">Physical path of your ASP.NET application</param>
        /// <returns></returns>
		public static FastCgiAspNetServer CreateApplicationHost(IPAddress address, int port, string virtualPath, string physicalPath, ILoggerFactory loggerFactory)
        {
            var ret = (FastCgiAspNetServer)ApplicationHost.CreateApplicationHost(typeof(FastCgiAspNetServer), virtualPath, physicalPath);
			ret.CreateServer (address, port, virtualPath, physicalPath, loggerFactory);
            return ret;
        }

		private void CreateServer(IPAddress address, int port, string virtualPath, string physicalPath, ILoggerFactory loggerFactory)
		{
			var channelFactory = new AspNetChannelFactory (loggerFactory, new AspNetRequestConfig {
				VirtualPath = virtualPath,
				PhysicalPath = physicalPath
			});

			_server = new TcpServer(address, port, channelFactory);
		}

        public override object InitializeLifetimeService()
        {
            return null; //never expire lease
        }

        /// <summary>
        /// Start listening for incoming connections
        /// </summary>
		public void Start()
		{
			_server.Start();
		}

        /// <summary>
        /// Stop listening for incoming connections
        /// </summary>
		public void Stop()
		{
			_server.Stop();
		}
    }	
}
