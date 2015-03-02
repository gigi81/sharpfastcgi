using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Pipes;
using ByteArray = Grillisoft.ImmutableArray.ImmutableArray<byte>;
using Grillisoft.FastCgi.Protocol;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Grillisoft.FastCgi.NamedPipes
{
    public abstract class NamedPipeServer : ILowerLayer
    {
        private readonly Stream _server;
        private readonly byte[] _recvBuffer = new byte[4096];
        private readonly byte[] _sendBuffer = new byte[4096];

        //private readonly Regex ParsePath = new Regex(@"\\\\([^\\]+)\\pipe\\([^\\]+)");

        private const uint STD_INPUT_HANDLE = 0xfffffff6;
        private const uint STD_OUTPUT_HANDLE = 0xfffffff5;
        private const uint STD_ERROR_HANDLE = 0xfffffff4;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);

        private static IntPtr StandardInputHandle { get { return GetStdHandle(STD_INPUT_HANDLE); } }

        public NamedPipeServer(string path)
        {
            //var match = ParsePath.Match(path);

            var handle = new SafeFileHandle(StandardInputHandle, true);
            _server = new FileStream(handle, FileAccess.ReadWrite, 4096, false);
        }

        /// <summary>
        /// Upper layer to send data received from tcp channel
        /// </summary>
        public IUpperLayer UpperLayer { get; set; }

        public void Run()
        {
            int read = 0;

            this.UpperLayer = this.CreateChannel(this);

            while(read >= 0)
            {
                read = _server.Read(_recvBuffer, 0, _recvBuffer.Length);
                if (read > 0)
                    this.UpperLayer.Receive(new ByteArray(_recvBuffer, (int)read));
            }

            _server.Close();
        }

        public void Send(ByteArray data)
        {
            lock(_sendBuffer)
            {
                while (data.Count > 0)
                {
                    int length = Math.Min(data.Count, _sendBuffer.Length);

                    data.CopyTo(_sendBuffer, 0, length);                    
                    _server.Write(_sendBuffer, 0, length);
                    data = data.SubArray(length, true);
                }

                _server.Flush();
            }
        }

        /// <summary>
        /// Creates a new FastCgiChannel
        /// </summary>
        /// <param name="tcpLayer">Lower <see cref="TcpLayer"/> used to communicate with the web server</param>
        protected abstract FastCgiChannel CreateChannel(ILowerLayer tcpLayer);
    }
}
