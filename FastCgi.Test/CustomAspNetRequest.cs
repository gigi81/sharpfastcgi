using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FastCgi.Protocol;
using FastCgi.AspNet;

namespace FastCgi.Test
{
	public class CustomAspNetRequest : AspNetRequest
	{
		public CustomAspNetRequest(ushort id, BeginRequestMessageBody body)
			: base(id, body)
		{
			//must be the same values used when calling ApplicationHost.CreateApplicationHost
			this.VirtualPath = "/";
			this.PhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Root");
		}
	}
}
