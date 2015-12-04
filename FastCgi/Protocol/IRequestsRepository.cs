using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Protocol
{
    public interface IRequestsRepository
    {
        void AddRequest(Request request);

        void RemoveRequest(Request request);

        Request GetRequest(ushort requestId);
    }
}
