using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Repositories
{
    public class SyncronizedRequestsRepository : IRequestsRepository
    {
        private readonly Dictionary<ushort, Request> _requests = new Dictionary<ushort, Request>();

        public void AddRequest(Protocol.Request request)
        {
            lock(_requests)
            {
                _requests.Add(request.Id, request);
            }
        }

        public void RemoveRequest(Protocol.Request request)
        {
            lock(_requests)
            {
                _requests.Remove(request.Id);
            }
        }

        public Protocol.Request GetRequest(ushort requestId)
        {
            lock(_requests)
            {
                return _requests[requestId];
            }
        }
    }
}
