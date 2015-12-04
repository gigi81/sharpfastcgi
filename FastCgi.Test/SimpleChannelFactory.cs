using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Test
{
    internal class SimpleChannelFactory : IFastCgiChannelFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public SimpleChannelFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Protocol.FastCgiChannel CreateChannel(Protocol.ILowerLayer layer)
        {
            return new SimpleChannel(layer, _loggerFactory);
        }
    }
}
