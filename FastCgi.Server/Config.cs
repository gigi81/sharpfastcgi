using System;
using System.IO;
using System.Net;
using System.Configuration;

namespace Grillisoft.FastCgi.Server
{
	internal class Config
	{
		private const string DefaultVirtualPath = "/";
		private const string DefaultPhysicalPath = "Root";
		private static IPAddress DefaultAddress = IPAddress.Any;
		private const int DefaultPort = 9000;

		public static string VirtualPath
		{
			get { return ConfigurationManager.AppSettings["VirtualPath"] ?? DefaultVirtualPath; }
		}

		public static string PhysicalPath
		{
			get
			{
				return Path.GetFullPath(ConfigurationManager.AppSettings["PhysicalPath"] ?? DefaultPhysicalPath);
			}
		}

		public static IPAddress Address
		{
			get
			{
				var value = ConfigurationManager.AppSettings ["Address"];

				IPAddress ret;
				if (!String.IsNullOrWhiteSpace(value) && IPAddress.TryParse(ConfigurationManager.AppSettings["Address"], out ret))
					return ret;

				return DefaultAddress;
			}
		}

		public static int Port
		{
			get
			{
				int ret;
				if (Int32.TryParse(ConfigurationManager.AppSettings["Port"], out ret))
					return ret;

				return DefaultPort;
			}
		}
	}
}

