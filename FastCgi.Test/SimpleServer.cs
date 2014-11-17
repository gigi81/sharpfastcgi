using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using FastCgi.Tcp;

namespace FastCgi.Test
{
    public class SimpleServer : CrossAppDomainObject
	{
        private readonly SimpleTcpServer _server;

        public SimpleServer(bool keepalive = false)
        {
            _server = new SimpleTcpServer(keepalive);
        }

		public void Start()
		{
			_server.Start();
		}

		public void Stop()
		{
			_server.Stop();
		}
	}
	
	internal class SimpleTcpServer : TcpServer
	{
		public const int DefaultPort = 9000;
        private readonly bool _keepalive = false;

		public SimpleTcpServer(bool keepalive = false)
			: this(DefaultPort, false)
		{
		}

		public SimpleTcpServer(int port, bool keepalive)
			: base(port)
		{
            _keepalive = keepalive;
		}

		protected override IChannel CreateChannel(TcpLayer tcpLayer)
		{
			return new SimpleChannelStack(tcpLayer, _keepalive, this.OnRequestEnded);
		}

        private void OnRequestEnded()
        {
        }

		private class SimpleChannelStack : IChannel
		{
			private readonly TcpLayer _tcpLayer;
            private readonly Action _endedCallback;
            private readonly bool _keepalive = false;

			public SimpleChannelStack(TcpLayer tcpLayer, bool keepalive, Action endedCallback = null)
			{
                _keepalive = keepalive;
                _endedCallback = endedCallback;
				_tcpLayer = tcpLayer;
			    _tcpLayer.UpperLayer = this.CreateUpperLayer(tcpLayer);
			}

            private SimpleChannel CreateUpperLayer(TcpLayer tcpLayer)
            {
                var channel = new SimpleChannel();
                channel.LowerLayer = tcpLayer;
                channel.RequestEnded += new EventHandler(RequestEnded);
                return channel;
            }

			private void RequestEnded(object sender, EventArgs e)
			{
                if(!_keepalive)
				    _tcpLayer.Close();

                if (_endedCallback != null)
                    _endedCallback.Invoke();
			}

            public void Run()
            {
                _tcpLayer.Run();
                _tcpLayer.Close();
            }
		}
	}

    /// <summary>
    /// Enables access to objects across application domain boundaries.
    /// Contrary to MarshalByRefObject, the lifetime is managed by the client.
    /// </summary>
    public abstract class CrossAppDomainObject : MarshalByRefObject
    {
        /// <summary>
        /// Count of remote references to this object.
        /// </summary>
        [NonSerialized]
        private int refCount;

        /// <summary>
        /// Disables LifeTime service : object has an infinite life time until it's Disconnected.
        /// </summary>
        /// <returns>null.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Connect a proxy to the object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AppDomainConnect()
        {
            int value = Interlocked.Increment(ref refCount);
            Debug.Assert(value > 0);
        }

        /// <summary>
        /// Disconnects a proxy from the object.
        /// When all proxy are disconnected, the object is disconnected from RemotingServices.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AppDomainDisconnect()
        {
            Debug.Assert(refCount > 0);
            if (Interlocked.Decrement(ref refCount) == 0)
                RemotingServices.Disconnect(this);
        }
    }
}
