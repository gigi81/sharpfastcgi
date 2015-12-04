using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Grillisoft.FastCgi.Servers;
using System.Configuration;

namespace Grillisoft.FastCgi.Server
{
    public class TcpServerConfig : ITcpServerConfig
    {
        public IPAddress Address
        {
			get { return Config.Address; }
        }

        public int Port
        {
			get { return Config.Port; }
        }

    }
}
