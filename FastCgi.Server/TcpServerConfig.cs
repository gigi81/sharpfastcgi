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
        private static IPAddress DefaultAddress = IPAddress.Any;
        private const int DefaultPort = 9000;

        public IPAddress Address
        {
            get
            {
                IPAddress ret;
                if (IPAddress.TryParse(ConfigurationManager.AppSettings["Address"], out ret))
                    return ret;

                return DefaultAddress;
            }
        }

        public int Port
        {
            get
            {
                int ret;
                if (Int32.TryParse(ConfigurationManager.AppSettings["Port"], out ret))
                    return ret;

                return DefaultPort;
            }
        }

    }
}
