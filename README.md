[![Build Status](https://ci.appveyor.com/api/projects/status/01w5s07237opjnj3?svg=true)](https://ci.appveyor.com/project/gigi81/sharpfastcgi)

sharpfastcgi
============

C# fastcgi protocol implementation plus some usage examples.
A good example on how to self-host your web application without the need of iis or mono.

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

Documentation
============
You can also read this article I wroute about this library so you can have a deeper understanding on how it works: http://www.codeproject.com/Articles/388040/FastCGI-NET-and-ASP-NET-self-hosting
