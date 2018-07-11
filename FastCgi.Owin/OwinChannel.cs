using Grillisoft.FastCgi.Protocol;
using Microsoft.Owin;

namespace Grillisoft.FastCgi.Owin
{
    public class OwinChannel : SimpleFastCgiChannel
    {
        OwinMiddleware _pipeline;

        public OwinChannel(ILowerLayer layer, ILoggerFactory loggerFactory, OwinMiddleware pipeline)
            : base(layer, loggerFactory)
        {
            _pipeline = pipeline;
        }

        protected override Request CreateRequest(ushort requestId, BeginRequestMessageBody body)
        {
            return new OwinRequest(requestId, body, _pipeline);
        }
    }
}
