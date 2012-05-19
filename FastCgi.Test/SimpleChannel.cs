using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastCgi.Test
{
    public class SimpleChannel : FastCgi.Protocol.FastCgiChannel
    {
        private Dictionary<ushort, SimpleRequest> _request = new Dictionary<ushort, SimpleRequest>();

        public SimpleChannel()
        {
            this.Properties = new Protocol.ChannelProperties()
            {
                MaximumConnections = 1,
                MaximumRequests = 1,
                SupportMultiplexedConnection = false
            };
        }

        protected override Protocol.Request CreateRequest(ushort requestId, Protocol.BeginRequestMessageBody body)
        {
            return new SimpleRequest(requestId, body);
        }

        protected override void AddRequest(Protocol.Request request)
        {
            _request.Add(request.Id, (SimpleRequest)request);
        }

        protected override void RemoveRequest(Protocol.Request request)
        {
            _request.Remove(request.Id);
        }

        protected override Protocol.Request GetRequest(ushort requestId)
        {
            return _request[requestId];
        }
    }
}
