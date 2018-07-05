using Grillisoft.FastCgi.Protocol;

namespace Grillisoft.FastCgi.Owin
{
    public class OwinChannel : SimpleFastCgiChannel
    {
        Microsoft.Owin.OwinMiddleware pipeline;

        public OwinChannel(ILowerLayer layer, ILoggerFactory loggerFactory, Microsoft.Owin.OwinMiddleware pipeline)
            : base(layer, loggerFactory)
        {
            this.pipeline = pipeline;
        }

        protected override Request CreateRequest(ushort requestId, BeginRequestMessageBody body)
        {
            return new OwinRequest(requestId, body, pipeline);
        }
    }
}
