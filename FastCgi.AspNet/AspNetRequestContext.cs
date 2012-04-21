using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using FastCgi.Protocol;

namespace FastCgi.AspNet
{
	[Serializable]
	public class AspNetRequestContext
	{
		public AspNetRequestContext()
		{
			this.Status = String.Empty;
			this.HeaderSent = false;
			this.Headers = new NameValueCollection();
		}

		public string Status { get; set; }

		public bool HeaderSent { get; set; }

		public string VirtualPath { get; set; }

		public string PhysicalPath { get; set; }

		public InputStream InputStream { get; set; }

		public OutputStream OutputStream { get; set; }

		public NameValueCollection Headers { get; set; }

		public NameValuePairCollection Parameters { get; set; }

		public void SetStatus(int statusCode, string statusDescription)
		{
			if (this.HeaderSent)
				throw new InvalidOperationException("Cannot set status on a response that has already been flushed");

			this.Status = String.Format("{0} {1} {2}", this.Parameters.GetValue("SERVER_PROTOCOL"), statusCode, statusDescription);
		}

		public void SetHeader(string name, string value)
		{
			if (this.HeaderSent)
				throw new InvalidOperationException("Cannot set headers on a response that has already been flushed");

			this.Headers.Set(name, value);
		}

		public void End()
		{
		}
	}
}
