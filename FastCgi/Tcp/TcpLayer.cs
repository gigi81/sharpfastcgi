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
        private TcpClient _client;

        byte[] _receiveBuffer = new byte[FastCgi.Protocol.Consts.SuggestedBufferSize];
        byte[] _sendBuffer = new byte[FastCgi.Protocol.Consts.SuggestedBufferSize];

        public TcpLayer(TcpClient client)
        {
            _client = client;
        }

		/// <summary>
		/// Upper layer to send data received from tcp channel
		/// </summary>
        public IUpperLayer UpperLayer { get; set; }

        public void Run()
        {
            while (_client.Connected)
            {
				int read = 0;

				try
				{
					//blocking call
					read = 0;
					read = _client.GetStream().Read(_receiveBuffer, 0, _receiveBuffer.Length);
				}
				catch (Exception ex)
				{
					//TODO: handle better
				}

				if (read > 0)
					this.UpperLayer.Receive(new ByteArray(_receiveBuffer, read));
            }
        }

        public void Send(ByteArray data)
        {
            lock (_client)
            {
                data.CopyTo(_sendBuffer, 0);
                _client.GetStream().Write(_sendBuffer, 0, data.Count);
                _client.GetStream().Flush();
            }
        }

        public void Close()
        {
            _client.Close();
        }
    }
}
