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
using System.Linq;
using System.Text;
using System.IO;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public abstract class Request
	{
		public event EventHandler Ended;
		public event EventHandler<FlushEventArgs> OutputFlushing;
		public event EventHandler<FlushEventArgs> ErrorFlushing;

		private NameValuePairCollection _parameters = null;

		protected Request(ushort id, BeginRequestMessageBody body)
		{
			this.Id = id;
			this.RequestBody = body;
			this.ExitCode = 0;

			this.ParametersStream = new InputStream();
			this.InputStream      = new InputStream();
			this.DataStream       = new InputStream();

			this.OutputStream     = new OutputStream();
			this.ErrorStream      = new OutputStream();

			this.OutputStream.Flushing += new EventHandler<FlushEventArgs>(OutputStreamFlushing);
			this.ErrorStream.Flushing += new EventHandler<FlushEventArgs>(ErrorStreamFlushing);
		}

		private void ErrorStreamFlushing(object sender, FlushEventArgs e)
		{
			this.OnErrorStreamFlushing(e);
		}

		private void OutputStreamFlushing(object sender, FlushEventArgs e)
		{
			this.OnOutputStreamFlushing(e);
		}

		public ushort Id { get; protected set; }

		public BeginRequestMessageBody RequestBody { get; protected set; }

		public int ExitCode { get; protected set; }

		public NameValuePairCollection Parameters
		{
			get
			{
				if(_parameters == null)
					_parameters = new NameValuePairCollection(this.ParametersStream);

				return _parameters;
			}
		}

		public InputStream ParametersStream { get; protected set; }

		public InputStream InputStream { get; protected set; }

		public InputStream DataStream { get; protected set; }

		public OutputStream OutputStream { get; protected set; }

		public OutputStream ErrorStream { get; protected set; }

		public abstract void Execute();

		public abstract void Abort();

		/// <summary>
		/// Ends the request
		/// </summary>
		public void End()
		{
			this.OutputStream.Flush();
			this.ErrorStream.Flush();

			if (this.Ended != null)
				this.Ended(this, EventArgs.Empty);
		}

		protected virtual void OnOutputStreamFlushing(FlushEventArgs args)
		{
			if (this.OutputFlushing != null)
				this.OutputFlushing(this, args);
		}

		protected virtual void OnErrorStreamFlushing(FlushEventArgs args)
		{
			if (this.ErrorFlushing != null)
				this.ErrorFlushing(this, args);
		}
	}
}
