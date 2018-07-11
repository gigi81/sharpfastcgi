using System;
using Grillisoft.FastCgi.Protocol;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;

namespace Grillisoft.FastCgi.Owin
{
    public class OwinChannelFactory : IFastCgiChannelFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly Action<IAppBuilder> _appInitializer;

        public OwinChannelFactory(ILoggerFactory loggerFactory, Action<IAppBuilder> appInitializer)
        {
            _loggerFactory = loggerFactory;
            _appInitializer = appInitializer;
        }

        public FastCgiChannel CreateChannel(ILowerLayer lowerLayer)
        {
            var appBuilder = new AppBuilder();
            appBuilder.Properties.Add(Constants.OwinVersion);
            _appInitializer(appBuilder);

            var pipeline = appBuilder.Build<OwinMiddleware>();
            return new OwinChannel(lowerLayer, _loggerFactory, pipeline);
        }
    }
}
