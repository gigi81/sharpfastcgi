using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Test
{
    public class SimpleChannel : Grillisoft.FastCgi.Protocol.SimpleFastCgiChannel
    {
        public SimpleChannel(ILowerLayer layer, ILoggerFactory loggerFactory)
            : base(layer, loggerFactory)
        {
        }

        protected override Protocol.Request CreateRequest(ushort requestId, Protocol.BeginRequestMessageBody body)
        {
            return new SimpleRequest(requestId, body);
        }
    }
}
