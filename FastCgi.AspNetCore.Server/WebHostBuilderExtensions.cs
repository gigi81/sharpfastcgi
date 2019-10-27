using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Grillisoft.FastCgi.AspNetCore.Server
{
    public static class WebHostBuilderKestrelExtensions
    {
        /// <summary>
        /// Specify FastCgi as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseFastCgi(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IFastCgiServer, >
                services.AddTransient<IConfigureOptions<FastCgiServerOptions>, FastCgiServerOptionsSetup>();
                services.AddSingleton<IServer, FastCgiServer>();
            });
        }

        /// <summary>
        /// Specify FastCgi as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <param name="options">
        /// A callback to configure Kestrel options.
        /// </param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseFastCgi(this IWebHostBuilder hostBuilder, Action<FastCgiServerOptions> options)
        {
            return hostBuilder.UseFastCgi().ConfigureServices(services =>
            {
                services.Configure(options);
            });
        }

        /// <summary>
        /// Specify FastCgi as the server to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.
        /// </param>
        /// <param name="configureOptions">A callback to configure Kestrel options.</param>
        /// <returns>
        /// The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseFastCgi(this IWebHostBuilder hostBuilder, Action<WebHostBuilderContext, FastCgiServerOptions> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            return hostBuilder.UseFastCgi().ConfigureServices((context, services) =>
            {
                services.Configure<FastCgiServerOptions>(options =>
                {
                    configureOptions(context, options);
                });
            });
        }
    }
}