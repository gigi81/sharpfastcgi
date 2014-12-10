#region License
//*****************************************************************************/
// Copyright (c) 2012 Luigi Grilli
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//*****************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using FastCgi.Protocol;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace FastCgi.AspNet
{
	public class AspNetRequest : Request
	{
		public AspNetRequest(ushort id, BeginRequestMessageBody body)
			: base(id, body)
		{
			this.Status = String.Empty;
			this.Headers = new NameValueCollection();
		}

		public string VirtualPath { get; set; }

		public string PhysicalPath { get; set; }

		public bool HeaderSent { get; protected set; }

		public string Status { get; protected set; }

		public NameValueCollection Headers { get; protected set; }

		public void SetStatus(int statusCode, string statusDescription)
		{
			if (this.HeaderSent)
				throw new InvalidOperationException("Cannot set status on a response that has already been flushed");

			this.Status = String.Format("Status: {0} {1}", statusCode, statusDescription);
		}

		public void SetHeader(string name, string value)
		{
			if (this.HeaderSent)
				throw new InvalidOperationException("Cannot set headers on a response that has already been flushed");

			this.Headers.Set(name, value);
		}

		protected override void OnOutputStreamFlushing(FlushEventArgs args)
		{
			if (!this.HeaderSent)
			{
				args = new FlushEventArgs(this.SerializeHeaders() + args.Data);
				this.HeaderSent = true;
			}

			base.OnOutputStreamFlushing(args);
		}

		public override void Execute()
		{
			System.Web.HttpRuntime.ProcessRequest(new FastCgiWorkerRequest(this));
		}

		/// <summary>
		/// This is single threaded so we can't abort the request
		/// </summary>
		public override void Abort()
		{
			throw new InvalidOperationException("Abort is not available");
		}

		protected virtual ByteArray SerializeHeaders()
		{
			StringBuilder builder = new StringBuilder();

			if (!String.IsNullOrEmpty(this.Status))
			{
				builder.Append(this.Status);
				builder.Append("\r\n");
			}

			foreach (string key in this.Headers.Keys)
			{
				builder.Append(key);
				builder.Append(": ");
				builder.Append(this.Headers[key]);
				builder.Append("\r\n");
			}

			builder.Append("\r\n");

            //TODO: can improve performance without creating the temporary array
			return new ByteArray(Encoding.UTF8.GetBytes(builder.ToString()));
		}
	}
}
