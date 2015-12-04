using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Repositories
{
    public class RequestsRepository : IRequestsRepository
    {
        private readonly Dictionary<ushort, Request> _requests = new Dictionary<ushort, Request>();

        public void AddRequest(Protocol.Request request)
        {
            _requests.Add(request.Id, request);
        }

        public void RemoveRequest(Protocol.Request request)
        {
            _requests.Remove(request.Id);
        }

        public Protocol.Request GetRequest(ushort requestId)
        {
            return _requests[requestId];
        }
    }
}
