using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Grillisoft.FastCgi.Servers
{
    public interface ITcpServerConfig
    {
        IPAddress Address { get; }

        int Port { get; }
    }
}
