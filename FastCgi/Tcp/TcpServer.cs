#region License
//*****************************************************************************/
// Copyright (c) 2012 Luigi Grilli
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//*****************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace FastCgi.Tcp
{
	/// <summary>
	/// TcpServer to listen for incoming fastcgi connections
	/// </summary>
	public abstract class TcpServer
	{
		public event EventHandler<UnhandledExceptionEventArgs> AcceptError;
		public event EventHandler<UnhandledExceptionEventArgs> ChannelError;

		private readonly TcpListener _listener;

		/// <summary>
		/// Creates a new TcpServer listening on 127.0.0.1 on the port specified
		/// </summary>
		/// <param name="port">TCP/IP port to listen for incoming connections</param>
		protected TcpServer(int port)
			: this(IPAddress.Any /* Ipv4 */, port)
		{
		}

		/// <summary>
		/// Creates a new TcpServer listening on the address and port specified
		/// </summary>
		/// <param name="address">Address to listen for incoming connections</param>
		/// <param name="port">TCP/IP port to listen for incoming connections</param>
		protected TcpServer(IPAddress address, int port)
		{
			_listener = new TcpListener(address, port);
		}

		/// <summary>
		/// Starts listening for incoming connections
		/// </summary>
		public void Start()
		{
			this.IsListening = true;

			_listener.Start();
			_listener.BeginAcceptTcpClient(this.AcceptConnection, null);
		}

		/// <summary>
		/// Stops listening for incoming connections
		/// </summary>
		public void Stop()
		{
			this.IsListening = false;
			_listener.Stop();
		}

		public bool IsListening { get; private set; }

		/// <summary>
		/// Asyncronous callback method to receive incoming connections
		/// Each connection will have it's own thread.
		/// </summary>
		/// <param name="result">Asyncronous result</param>
		private void AcceptConnection(IAsyncResult result)
		{
			TcpClient client = null;

			try
			{
				client = _listener.EndAcceptTcpClient(result);
			}
			catch (Exception ex)
			{
				this.OnAcceptError(new UnhandledExceptionEventArgs(ex, false));
			}

			if(this.IsListening)
				_listener.BeginAcceptTcpClient(this.AcceptConnection, null);

			//this is a blocking call
			if (client != null)
				this.RunChannel(client);
		}

		private void RunChannel(TcpClient client)
		{
			try
			{
				var channel = this.CreateChannel(new TcpLayer(client));
				channel.Run();
			}
			catch (Exception ex)
			{
				this.OnChannelError(new UnhandledExceptionEventArgs(ex, false));
			}
		}

		/// <summary>
		/// Creates a new FastCgiChannel
		/// </summary>
		/// <param name="tcpLayer">Lower <see cref="TcpLayer"/> used to communicate with the web server</param>
		protected abstract IChannel CreateChannel(TcpLayer tcpLayer);

		protected virtual void OnAcceptError(UnhandledExceptionEventArgs args)
		{
			if (this.AcceptError != null)
				this.AcceptError(this, args);
		}

		protected virtual void OnChannelError(UnhandledExceptionEventArgs args)
		{
			if (this.ChannelError != null)
				this.ChannelError(this, args);
		}
	}

    public interface IChannel
    {
        void Run();
    }
}
