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

