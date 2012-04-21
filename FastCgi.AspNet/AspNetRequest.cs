using System;
using System.Collections.Generic;
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
		private AspNetRequestContext _context;

		public AspNetRequest(ushort id, BeginRequestMessageBody body)
			: base(id, body)
		{
		}

		public bool HeaderSent { get; protected set; }

		public string VirtualPath { get; set; }

		public string PhysicalPath { get; set; }

		protected AspNetRequestContext Context
		{
			get
			{
				if (_context == null)
				{
					_context = new AspNetRequestContext();
					_context.InputStream = this.InputStream;
					_context.OutputStream = this.OutputStream;
					_context.Parameters = this.Parameters;
					_context.PhysicalPath = this.PhysicalPath;
					_context.VirtualPath = this.VirtualPath;
					//_context.Ended += new EventHandler(_context_Ended);
				}

				return _context;
			}
		}

		void _context_Ended(object sender, EventArgs e)
		{
			this.End();
		}

		//protected Thread Thread { get; set; }

		protected override void OnOutputStreamFlushing(FlushEventArgs args)
		{
			if (!this.Context.HeaderSent)
			{
				args = new FlushEventArgs(this.SerializeHeaders() + args.Data);
				this.Context.HeaderSent = true;
			}

			base.OnOutputStreamFlushing(args);
		}

		public override void Execute()
		{
			//this.Thread = new System.Threading.Thread(this.ProcessRequest);
			//this.Thread.Start();
			ProcessRequest();
		}

		protected virtual void ProcessRequest()
		{
			//MyExeHost myHost = (MyExeHost)MyAspHost.CreateApplicationHost(typeof(MyExeHost), this.VirtualPath, this.PhysicalPath);
			MyExeHost myHost = (MyExeHost)ApplicationHost.CreateApplicationHost(typeof(MyExeHost), this.VirtualPath, this.PhysicalPath);
			myHost.SetContext(this.Context);
			myHost.ProcessRequest();
			this.End();
		}

		public override void Abort()
		{
			//if(this.Thread != null && this.Thread.IsAlive)
			//    this.Thread.Abort();
		}

		protected virtual ByteArray SerializeHeaders()
		{
			StringBuilder builder = new StringBuilder();

			if (String.IsNullOrEmpty(this.Context.Status))
			{
				builder.Append(this.Context.Status);
				builder.Append("\r\n");
			}

			foreach (string key in this.Context.Headers.Keys)
			{
				builder.Append(key);
				builder.Append(": ");
				builder.Append(this.Context.Headers[key]);
				builder.Append("\r\n");
			}

			builder.Append("\r\n");

			return new ByteArray(Encoding.UTF8.GetBytes(builder.ToString()));
		}
	}
}
