using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grillisoft.FastCgi.Protocol;

namespace Grillisoft.FastCgi.AspNet
{
    public class AspNetChannel : FastCgiChannel
    {
        private readonly IAspNetRequestConfig _config;

        public AspNetChannel(ILowerLayer lowerLayer, ILoggerFactory loggerFactory, IAspNetRequestConfig config)
            : base(lowerLayer, new Repositories.SyncronizedRequestsRepository(), loggerFactory)
        {
            _config = config;
        }

        protected override Request CreateRequest(ushort requestId, BeginRequestMessageBody body)
        {
            return new AspNetRequest(requestId, body, _config);
        }
    }
}
