using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Web.Hosting;
using Grillisoft.FastCgi.Protocol;
using Grillisoft.FastCgi.Servers;

namespace Grillisoft.FastCgi.AspNet
{
    public class FastCgiAspNetServer : MarshalByRefObject
    {
        //private ServerInternal _server;

        /// <summary>
        /// Create a fastcgi server
        /// </summary>
        /// <param name="port">Tcp/ip port where the server will be listening for fastcgi requests</param>
        /// <param name="virtualPath">Virtual path of your ASP.NET application</param>
        /// <param name="physicalPath">Physical path of your ASP.NET application</param>
        /// <returns></returns>
        public static FastCgiAspNetServer CreateApplicationHost(int port, string virtualPath, string physicalPath)
        {
            var ret = (FastCgiAspNetServer)ApplicationHost.CreateApplicationHost(typeof(FastCgiAspNetServer), virtualPath, physicalPath);
            //ret.Initialize(port, virtualPath, physicalPath);
            return ret;
        }

        public override object InitializeLifetimeService()
        {
            return null; //never expire lease
        }

        internal void Initialize(int port, string virtualPath, string physicalPath)
        {
            //_server = new ServerInternal(port, virtualPath, physicalPath);
        }

        /// <summary>
        /// Start listening for incoming connections
        /// </summary>
		public void Start()
		{
			//_server.Start();
		}

        /// <summary>
        /// Stop listening for incoming connections
        /// </summary>
		public void Stop()
		{
			//_server.Stop();
		}
    }	
}
