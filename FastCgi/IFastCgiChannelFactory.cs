using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi
{
    public interface IFastCgiChannelFactory
    {
        /// <summary>
        /// Creates a new FastCgiChannel
        /// </summary>
        /// <param name="tcpLayer">Lower <see cref="TcpLayer"/> used to communicate with the web server</param>
        FastCgiChannel CreateChannel(ILowerLayer tcpLayer);
    }
}
