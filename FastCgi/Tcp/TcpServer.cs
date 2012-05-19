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
using System.Net.Sockets;

namespace FastCgi.Tcp
{
    public abstract class TcpServer
    {
        private TcpListener _listener;

        public TcpServer(int port)
        {
            _listener = new TcpListener(port);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginAcceptTcpClient(this.AcceptConnection, null);
        }

        private void AcceptConnection(IAsyncResult result)
        {
            TcpClient client;

            try
            {
                client = _listener.EndAcceptTcpClient(result);
            }
            catch (Exception ex)
            {
                client = null;
            }

            _listener.BeginAcceptTcpClient(this.AcceptConnection, null);

            TcpLayer tcpLayer = new TcpLayer(client);
            this.CreateChannel(tcpLayer);
            tcpLayer.Run();
        }

		/// <summary>
		/// Creates a new FastCgiChannel
		/// </summary>
		/// <param name="tcpLayer"></param>
        protected abstract void CreateChannel(TcpLayer tcpLayer);
    }
}
