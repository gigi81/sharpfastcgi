[![Build Status](https://ci.appveyor.com/api/projects/status/01w5s07237opjnj3?svg=true)](https://ci.appveyor.com/project/gigi81/sharpfastcgi)

sharpfastcgi
============

C# fastcgi protocol implementation plus some usage examples.
A good example on how to self-host your web application without the need of IIS or Mono.

The purpose of this implementation is to have a more reliable solution than the one
offered with Mono and also a cleaner and reusable implementation of the protocol
to host not only ASP.NET applications but also custom low level (and fast) applications,
the ones that usually are implemented with an HttpListener.
With this implementation is possible for example to host an ASP.NET application (also MVC)
with and Nginx web-server on both Windows and Linux.

Example (Nginx)
============
You can run the first example/test following this procedure:

- under Windows run nginx.exe included in the Examples folder or under Linux run nginx with the configuration supplied with the Windows example
- start FastCgi.Test exe within visual studio or from a command prompt (in this case you will need to build it first)
- with a browser goto http://localhost:8082/info.aspx or http://localhost:8082/test.aspx

Example (Owin)
==============

sharpfastcgi now has an Owin-compatible ChannelFactory that allows you to run any
Owin-compatible middleware as a FastCGI application.  It has been tested with Microsoft 
WebAPI 5.2.3 and with the various Microsoft security middlewares for authentication to
ADFS, Facebook, Twitter, etc.

A minimal example for IIS would look like this:

```c#
    static class Program
    {
        static int Main(string[] args)
        {
            var logger = LoggerFactory.Create("Owin Test");
            logger.Log(LogLevel.Info, "Starting fastcgi server");

            var server = CreateServer();
            server.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static IFastCgiServer CreateServer()
        {
            return new IisServer(new OwinChannelFactory(LoggerFactory, applicationRegistration), LoggerFactory);
        }

        private static ILoggerFactory _loggerFactory;

        private static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory != null)
                    return _loggerFactory;

                return _loggerFactory = new Grillisoft.FastCgi.Loggers.Log4Net.LoggerFactory();
            }
        }

        static void applicationRegistration(IAppBuilder app)
        {
			// Register your Owin middlewares here
			// This example uses ADFS Bearer Auth and WebAPI

            var configuration = new HttpConfiguration();

            app.UseActiveDirectoryFederationServicesBearerAuthentication(
                new ActiveDirectoryFederationServicesBearerAuthenticationOptions
                {
                    MetadataEndpoint ="MyMetadataEndpoint",
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = "MyAudience"
                    }
                });

            app.UseWebApi(configuration);
        }
    }
```

Documentation
============
You can also read this article I wrote about this library so you can have a deeper understanding of how it works: http://www.codeproject.com/Articles/388040/FastCGI-NET-and-ASP-NET-self-hosting
