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
using System.IO;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace FastCgi.AspNet
{
	public class FastCgiWorkerRequest : System.Web.Hosting.SimpleWorkerRequest
	{
		private readonly AspNetRequest _request;

		public FastCgiWorkerRequest(AspNetRequest request)
			: base(String.Empty, String.Empty, null)
		{
			if (null == request)
				throw new ArgumentNullException("request");

			_request = request;
		}

		public override void EndOfRequest()
		{
			_request.End();
		}

		public override void FlushResponse(bool finalFlush)
		{
			_request.OutputStream.Flush();
		}

		public override string GetHttpVerbName()
		{
			return _request.Parameters.GetValue("REQUEST_METHOD");
		}

		public override string GetHttpVersion()
		{
			return _request.Parameters.GetValue("SERVER_PROTOCOL");
		}

		public override string GetLocalAddress()
		{
			return _request.Parameters.GetValue("SERVER_ADDR");
		}

		public override int GetLocalPort()
		{
			return Int32.Parse(_request.Parameters.GetValue("SERVER_PORT"));
		}

		public override string GetQueryString()
		{
			return _request.Parameters.GetValue("QUERY_STRING");
		}

		public override string GetRawUrl()
		{
			return _request.Parameters.GetValue("REQUEST_URI");
		}

		public override string GetRemoteAddress()
		{
			return _request.Parameters.GetValue("REMOTE_ADDR");
		}

		public override int GetRemotePort()
		{
			return Int32.Parse(_request.Parameters.GetValue("REMOTE_PORT"));
		}

		public override string GetUriPath()
		{
			return _request.Parameters.GetValue("REQUEST_URI");
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			_request.SetHeader(GetKnownResponseHeaderName(index), value);
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			_request.OutputStream.Write(data, 0, length);
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			_request.SetStatus(statusCode, statusDescription);
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			_request.SetHeader(name, value);
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			using (SafeFileHandle sfh = new SafeFileHandle(handle, true))
			{
				using (Stream stream = new FileStream(sfh, FileAccess.Read))
				{
					byte[] buffer = new byte[length];
					int read = stream.Read(buffer, (int)offset, buffer.Length);
					_request.OutputStream.Write(buffer, 0, read);
				}
			}
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			using (Stream stream = File.OpenRead(filename))
			{
				byte[] buffer = new byte[length];
				int read = stream.Read(buffer, (int)offset, buffer.Length);
				_request.OutputStream.Write(buffer, 0, read);
			}
		}

		public override string GetAppPath()
		{
			return _request.VirtualPath;
		}

		public override string GetAppPathTranslated()
		{
			return _request.PhysicalPath;
		}

		public override int ReadEntityBody(byte[] buffer, int size)
		{
			return _request.InputStream.Read(buffer, 0, size);
		}

		public override string GetUnknownRequestHeader(string name)
		{
			return _request.Parameters.GetValue(HeaderNameToParameterName(name));
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			return _request.Parameters
						   .Where(p => p.Name.StartsWith("HTTP_"))
						   .Where(p => !IsKnownRequestHeader(ParameterNameToHeaderName(p.Name)))
						   .Select(p => new[] { p.Name, p.Value })
                           .ToArray();
		}

		public static bool IsKnownRequestHeader(string header)
		{
			return GetKnownRequestHeaderIndex(header) >= 0;
		}

		public override string GetKnownRequestHeader(int index)
		{
			return _request.Parameters.GetValue(HeaderNameToParameterName(GetKnownRequestHeaderName(index)));
		}

		public override string GetServerVariable(string name)
		{
			return _request.Parameters.GetValue(name);
		}

		public override string GetFilePath()
		{
			return _request.Parameters.GetValue("SCRIPT_NAME").Replace('/', Path.DirectorySeparatorChar);
		}

		public override string GetFilePathTranslated()
		{
			return _request.PhysicalPath + this.GetFilePath();
		}

		public override string GetPathInfo()
		{
			//WARNING: the webserver must be configured to set the PATH_INFO fastcgi parameter
			return _request.Parameters.GetValue("PATH_INFO") ?? String.Empty;
		}

		private string HeaderNameToParameterName(string headerName)
		{
			return "HTTP_" + headerName.ToUpper().Replace('-', '_');
		}

		private string ParameterNameToHeaderName(string parameterName)
		{
			if (!parameterName.StartsWith("HTTP_"))
				throw new ArgumentException("parameterName");

			return parameterName.Substring(5).Replace('_', '-');
		}
	}
}
