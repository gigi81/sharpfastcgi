using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

namespace FastCgi.NamedPipes
{
    public class NamedPipeServer
    {
        private readonly NamedPipeServerStream _server;

        public NamedPipeServer(string pipeName)
        {
            _server = new NamedPipeServerStream(pipeName);
        }

        private void Run()
        {

        }
    }
}
