using System;
using System.Collections.Generic;
using System.Net.Sockets;
using FastCgi.Protocol;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Tcp
{
    public class TcpLayer : ILowerLayer
    {
        private Queue<ByteArray> _queue = new Queue<ByteArray>();
        private TcpClient _client;

        byte[] _receiveBuffer = new byte[FastCgi.Protocol.Consts.SuggestedBufferSize];
        byte[] _sendBuffer = new byte[FastCgi.Protocol.Consts.SuggestedBufferSize];

        public TcpLayer(TcpClient client)
        {
            _client = client;
        }

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
