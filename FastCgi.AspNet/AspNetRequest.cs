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
using System.Collections.Specialized;
using Grillisoft.FastCgi.Protocol;
using ByteArray = Grillisoft.ImmutableArray.ImmutableArray<byte>;
using Grillisoft.ImmutableArray;
using System.IO;
using System.Text;

namespace Grillisoft.FastCgi.AspNet
{
    public class AspNetRequest : Request
	{
        private readonly IAspNetRequestConfig _config;

        public AspNetRequest(ushort id, BeginRequestMessageBody body, IAspNetRequestConfig config)
			: base(id, body)
		{
            _config = config;
			this.Status = String.Empty;
			this.Headers = new NameValueCollection();
		}

        public string VirtualPath { get { return _config.VirtualPath; } }

        public string PhysicalPath { get { return _config.PhysicalPath; } }

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
            using (var stream = new StreamToImmutable())
            {
                using (var builder = new StreamWriter(stream, Encoding.UTF8))
                {
                    if (!String.IsNullOrEmpty(this.Status))
                    {
                        builder.Write(this.Status);
                        builder.Write("\r\n");
                    }

                    foreach (string key in this.Headers.Keys)
                    {
                        builder.Write(key);
                        builder.Write(": ");
                        builder.Write(this.Headers[key]);
                        builder.Write("\r\n");
                    }

                    builder.Write("\r\n");

                    return stream.ToImmutableArray();
                }
            }
        }
	}
}
