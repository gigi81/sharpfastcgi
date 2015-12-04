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

namespace Grillisoft.FastCgi.Servers
{
    public class IisServer : IFastCgiServer, ILowerLayer
    {
        private readonly Stream _server;
        private readonly Thread _thread;
        private readonly IFastCgiChannelFactory _channelFactory;
        private readonly ILogger _logger;
        private readonly byte[] _recvBuffer = new byte[4096];
        private readonly byte[] _sendBuffer = new byte[4096];
        private bool _running = false;

        //private readonly Regex ParsePath = new Regex(@"\\\\([^\\]+)\\pipe\\([^\\]+)");

        private const uint STD_INPUT_HANDLE = 0xfffffff6;
        private const uint STD_OUTPUT_HANDLE = 0xfffffff5;
        private const uint STD_ERROR_HANDLE = 0xfffffff4;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);

        private static IntPtr StandardInputHandle { get { return GetStdHandle(STD_INPUT_HANDLE); } }

        public IisServer(IFastCgiChannelFactory channelFactory, ILoggerFactory loggerFactory)
        {
            var handle = new SafeFileHandle(StandardInputHandle, true);

            _channelFactory = channelFactory;
            _logger = loggerFactory.Create(this.GetType());

            _server = new FileStream(handle, FileAccess.ReadWrite, 4096, false);
            _thread = new Thread(this.Run);
        }

        /// <summary>
        /// Upper layer to send data received from tcp channel
        /// </summary>
        public IUpperLayer UpperLayer { get; set; }

        public void Start()
        {
            _logger.Log(LogLevel.Info, "Starting IIS server");
            _running = true;
            _thread.Start();
        }

        public void Stop()
        {
            _logger.Log(LogLevel.Info, "Stopping IIS server");
            _running = false;
            _thread.Abort();
        }

        private void Run()
        {
            try
            {
                this.UpperLayer = _channelFactory.CreateChannel(this);

                int read = 0;

                while (_running && read >= 0)
                {
                    read = _server.Read(_recvBuffer, 0, _recvBuffer.Length);
                    if (read > 0)
                        this.UpperLayer.Receive(new ByteArray(_recvBuffer, (int)read));
                }
            }
            finally
            {
                _server.Close();
            }
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
   }
}
