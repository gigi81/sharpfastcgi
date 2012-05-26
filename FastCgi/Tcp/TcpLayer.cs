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
using System.Net.Sockets;
using FastCgi.Protocol;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Tcp
{
	public class TcpLayer : ILowerLayer
	{
		public event EventHandler<UnhandledExceptionEventArgs> RunError;

		private TcpClient _client;

		byte[] _receiveBuffer;
		byte[] _sendBuffer;

		public TcpLayer(TcpClient client)
			: this(client, Consts.SuggestedBufferSize, Consts.SuggestedBufferSize)
		{
		}

		public TcpLayer(TcpClient client, int recvBufferSize, int sendBufferSize)
		{
			_client = client;
			_receiveBuffer = new byte[recvBufferSize];
			_sendBuffer = new byte[sendBufferSize];
		}

		/// <summary>
		/// Upper layer to send data received from tcp channel
		/// </summary>
		public IUpperLayer UpperLayer { get; set; }

		/// <summary>
		/// Continuously calls the <see cref="TcpClient.Read"/> method of the <see cref="TcpClient"/> while the socket is connected
		/// </summary>
		/// <remarks>This is a blocking call. When exiting this call the socket will be already closed</remarks>
		public void Run()
		{
			while (_client.Connected)
			{
				int read = 0;

				try
				{
					//blocking call
					read = _client.GetStream().Read(_receiveBuffer, 0, _receiveBuffer.Length);
					if (read > 0)
						this.UpperLayer.Receive(new ByteArray(_receiveBuffer, read));
				}
				catch (Exception ex)
				{
					this.OnRunError(new UnhandledExceptionEventArgs(ex, false));
				}
			}

			_client.Close();
		}

		/// <summary>
		/// Sends data to the network
		/// </summary>
		/// <param name="data">Data to send</param>
		public void Send(ByteArray data)
		{
			lock (_client)
			{
				while (data.Count > 0)
				{
					int length = Math.Min(data.Count, _sendBuffer.Length);

					data.CopyTo(_sendBuffer, 0, length);
					_client.GetStream().Write(_sendBuffer, 0, data.Count);
					_client.GetStream().Flush();
					data = data.SubArray(length);
				}
			}
		}

		public void Close()
		{
			_client.Close();
		}

		protected virtual void OnRunError(UnhandledExceptionEventArgs args)
		{
			if (this.RunError != null)
				this.RunError(this, args);
		}
	}
}
