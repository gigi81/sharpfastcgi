using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using FastCgi.Protocol;

namespace FastCgi.AspNet
{
	public class FastCgiWorkerRequest : System.Web.Hosting.SimpleWorkerRequest
	{
		private AspNetRequestContext _context;

		public FastCgiWorkerRequest(AspNetRequestContext ctx)
			: base(String.Empty, String.Empty, null)
		{
			if (null == ctx)
				throw new ArgumentNullException("ctx");

			_context = ctx;
		}

		public override void EndOfRequest()
		{
			_context.End();
		}

		public override void FlushResponse(bool finalFlush)
		{
			_context.OutputStream.Flush();
		}

		public override string GetHttpVerbName()
		{
			return _context.Parameters.GetValue("REQUEST_METHOD");
		}

		public override string GetHttpVersion()
		{
			return _context.Parameters.GetValue("SERVER_PROTOCOL");
		}

		public override string GetLocalAddress()
		{
			return _context.Parameters.GetValue("SERVER_ADDR");
		}

		public override int GetLocalPort()
		{
			return Int32.Parse(_context.Parameters.GetValue("SERVER_PORT"));
		}

		public override string GetQueryString()
		{
			return _context.Parameters.GetValue("QUERY_STRING");
		}

		public override string GetRawUrl()
		{
			return _context.Parameters.GetValue("REQUEST_URI");
		}

		public override string GetRemoteAddress()
		{
			return _context.Parameters.GetValue("REMOTE_ADDR");
		}

		public override int GetRemotePort()
		{
			return Int32.Parse(_context.Parameters.GetValue("REMOTE_PORT"));
		}

		public override string GetUriPath()
		{
			return _context.Parameters.GetValue("REQUEST_URI");
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			_context.SetHeader(HttpWorkerRequest.GetKnownResponseHeaderName(index), value);
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			_context.OutputStream.Write(data, 0, length);
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			_context.SetStatus(statusCode, statusDescription);
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			_context.SetHeader(name, value);
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			//TODO: implement me!
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			using (Stream s = File.OpenRead(filename))
			{
				byte[] buffer = new byte[length];
				int read = s.Read(buffer, (int)offset, buffer.Length);
				_context.OutputStream.Write(buffer, 0, read);
			}
		}

		public override string GetAppPath()
		{
			return _context.VirtualPath;
		}

		public override string GetAppPathTranslated()
		{
			return _context.PhysicalPath;
		}

		public override int ReadEntityBody(byte[] buffer, int size)
		{
			return _context.InputStream.Read(buffer, 0, size);
		}

		public override string GetUnknownRequestHeader(string name)
		{
			return _context.Parameters.GetValue(HeaderNameToParameterName(name));
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			//TODO: implement me
			return new string[0][];
		}

		public override string GetKnownRequestHeader(int index)
		{
			return _context.Parameters.GetValue(HeaderNameToParameterName(GetKnownRequestHeaderName(index)));
		}

		public override string GetServerVariable(string name)
		{
			return _context.Parameters.GetValue(name);
		}

		public override string GetFilePath()
		{
			return _context.Parameters.GetValue("SCRIPT_NAME").Replace('/', '\\');
		}

		public override string GetFilePathTranslated()
		{
			return _context.PhysicalPath + GetFilePath();
		}

		public override string GetPathInfo()
		{
			//TODO: implement me
			return String.Empty;
		}

		private string HeaderNameToParameterName(string headerName)
		{
			return "HTTP_" + headerName.ToUpper().Replace('-', '_');
		}
	}
}
