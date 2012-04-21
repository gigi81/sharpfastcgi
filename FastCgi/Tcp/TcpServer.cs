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

        protected abstract void CreateChannel(TcpLayer tcpLayer);
    }
}
