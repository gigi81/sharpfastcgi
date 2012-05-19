using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastCgi.Protocol
{
	/// <summary>
	/// A <see cref="FastCgiChannel"/> that can handle only one request at a time
	/// </summary>
	/// <typeparam name="TRequest">Request type handled by the channel</typeparam>
	public abstract class SimpleFastCgiChannel<TRequest> : FastCgiChannel where TRequest : Request
	{
		public SimpleFastCgiChannel()
		{
			this.Properties = new Protocol.ChannelProperties()
			{
				MaximumConnections = 1,
				MaximumRequests = 1,
				SupportMultiplexedConnection = false
			};
		}

		protected TRequest Request { get; set; }

		protected override void AddRequest(Request request)
		{
			this.Request = (TRequest)request;
		}

		protected override void RemoveRequest(Request request)
		{
			this.Request = null;
		}

		protected override Request GetRequest(ushort requestId)
		{
			return this.Request;
		}
	}
}
