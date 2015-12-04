using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Repositories
{
    public class SingleRequestRepository : IRequestsRepository
    {
        private Request _request;

		public void AddRequest(Request request)
		{
            _request = request;
		}

		public void RemoveRequest(Request request)
		{
            _request = null;
		}

		public Request GetRequest(ushort requestId)
		{
            return _request;
		}
    }
}
