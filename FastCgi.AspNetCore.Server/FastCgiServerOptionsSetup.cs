using System;
using Microsoft.Extensions.Options;

namespace Grillisoft.FastCgi.AspNetCore.Server
{
    public class FastCgiServerOptionsSetup : IConfigureOptions<FastCgiServerOptions>
    {
        private IServiceProvider _services;

        public FastCgiServerOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(FastCgiServerOptions options)
        {
            options.ApplicationServices = _services;
        }
    }
}
