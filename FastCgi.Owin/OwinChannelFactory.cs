using System;
using Grillisoft.FastCgi;
using Grillisoft.FastCgi.Protocol;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

namespace Grillisoft.FastCgi.Owin
{
    public class OwinChannelFactory : IFastCgiChannelFactory
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly Action<IAppBuilder> appInitializer;

        public OwinChannelFactory(ILoggerFactory loggerFactory, Action<IAppBuilder> appInitializer)
        {
            this.loggerFactory = loggerFactory;
            this.appInitializer = appInitializer;
        }

        public FastCgiChannel CreateChannel(ILowerLayer lowerLayer)
        {
            IAppBuilder appBuilder = new AppBuilder();
            appBuilder.Properties.Add(Constants.OwinVersion);
            appInitializer(appBuilder);

            OwinMiddleware pipeline = appBuilder.Build<OwinMiddleware>();
            return new OwinChannel(lowerLayer, loggerFactory, pipeline);
        }
    }
}
