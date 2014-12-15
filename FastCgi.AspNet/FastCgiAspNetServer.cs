using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Web.Hosting;
using FastCgi.Protocol;
using FastCgi.Tcp;

namespace FastCgi.AspNet
{
    public class FastCgiAspNetServer : MarshalByRefObject
    {
        private ServerInternal _server;

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
            ret.Initialize(port, virtualPath, physicalPath);
            return ret;
        }

        public override object InitializeLifetimeService()
        {
            return null; //never expire lease
        }

        internal void Initialize(int port, string virtualPath, string physicalPath)
        {
            _server = new ServerInternal(port, virtualPath, physicalPath);
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
	
	internal class ServerInternal : TcpServer
	{
        internal ServerInternal(int port, string virtualPath, string physicalPath)
			: base(port)
        {
            this.VirtualPath = virtualPath;
            this.PhysicalPath = physicalPath;
        }

        public string VirtualPath { get; private set; }

        public string PhysicalPath { get; private set; }

		protected override IChannel CreateChannel(TcpLayer tcpLayer)
		{
			return new CustomAspNetChannelStack(tcpLayer, this);
		}

        private class CustomAspNetChannelStack : IChannel
        {
            private readonly ServerInternal _server;
			private readonly TcpLayer _tcpLayer;

            public CustomAspNetChannelStack(TcpLayer tcpLayer, ServerInternal server)
			{
			    _server = server;
				_tcpLayer = tcpLayer;
				_tcpLayer.UpperLayer = this.CreateUpperLayer(tcpLayer);
			}

            private CustomAspNetChannel CreateUpperLayer(TcpLayer tcpLayer)
            {
                var channel = new CustomAspNetChannel(_server);
                channel.LowerLayer = tcpLayer;
                channel.RequestEnded += new EventHandler(RequestEnded);
                return channel;
            }

			private void RequestEnded(object sender, EventArgs e)
			{
                if (!((Request)sender).RequestBody.KeepConnection)
				    _tcpLayer.Close();
			}

            public void Run()
            {
                _tcpLayer.Run();
            }
		}

        internal class CustomAspNetChannel : SimpleFastCgiChannel<CustomAspNetRequest>
        {
            private readonly ServerInternal _server;

            internal CustomAspNetChannel(ServerInternal server)
            {
                _server = server;
            }

            protected override Request CreateRequest(ushort requestId, Protocol.BeginRequestMessageBody body)
            {
                return new CustomAspNetRequest(requestId, body, _server);
            }
        }

        internal class CustomAspNetRequest : AspNetRequest
        {
            public CustomAspNetRequest(ushort id, BeginRequestMessageBody body, ServerInternal server)
                : base(id, body)
            {
                //these must be the same values used when calling ApplicationHost.CreateApplicationHost
                this.VirtualPath = server.VirtualPath;
                this.PhysicalPath = server.PhysicalPath;
            }
        }
	}
}
