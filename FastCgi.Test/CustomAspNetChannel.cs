using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCgi.Protocol;

namespace FastCgi.Test
{
	public class CustomAspNetChannel : SimpleFastCgiChannel<CustomAspNetRequest>
	{
		protected override Request CreateRequest(ushort requestId, Protocol.BeginRequestMessageBody body)
		{
			return new CustomAspNetRequest(requestId, body);
		}
	}
}
