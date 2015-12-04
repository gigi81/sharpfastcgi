using System;
using Grillisoft.FastCgi;
using Grillisoft.FastCgi.Protocol;
using Grillisoft.FastCgi.AspNet;

namespace Grillisoft.FastCgi.AspNet
{
	public class AspNetChannelFactory : IFastCgiChannelFactory
	{
		ILoggerFactory _loggerFactory;
		IAspNetRequestConfig _requestConfig;

		public AspNetChannelFactory (ILoggerFactory loggerFactory, IAspNetRequestConfig requestConfig)
		{
			_loggerFactory = loggerFactory;
			_requestConfig = requestConfig;
		}

		/// <summary>
		/// Creates a new FastCgiChannel
		/// </summary>
		/// <param name="tcpLayer">Lower <see cref="TcpLayer"/> used to communicate with the web server</param>
		public FastCgiChannel CreateChannel(ILowerLayer tcpLayer)
		{
			return new AspNetChannel(tcpLayer, _loggerFactory, _requestConfig);
		}
	}

	public class AspNetRequestConfig : IAspNetRequestConfig
	{
		public string VirtualPath { get; set; }

		public string PhysicalPath { get; set ; }
	}
}

