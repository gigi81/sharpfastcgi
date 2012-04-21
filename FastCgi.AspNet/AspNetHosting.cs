using System;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Runtime.Remoting;
using System.Globalization;

namespace FastCgi.AspNet
{
	public class MyExeHost : MarshalByRefObject
	{
		private AspNetRequestContext _context;

		public void SetContext(AspNetRequestContext context)
		{
			_context = context;		
		}

		public void ProcessRequest()
		{
			System.Web.HttpRuntime.ProcessRequest(new FastCgiWorkerRequest(_context));
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}

	public class MyAspHost
	{
		public static object CreateApplicationHost(Type hostType, string virtualDir, string physicalDir)
		{
			if (!(physicalDir.EndsWith("\\")))
				physicalDir = physicalDir + "\\";

			string aspDir = HttpRuntime.AspInstallDirectory;
			string domainId = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo).GetHashCode().ToString("x");
			string appName = (virtualDir + physicalDir).GetHashCode().ToString("x");

			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationName = appName;
			setup.ConfigurationFile = "web.config";  // not necessary execept for debugging
			AppDomain ad = AppDomain.CreateDomain(domainId, null, setup);
			ad.SetData(".appDomain", "*");
			ad.SetData(".appPath", physicalDir);
			ad.SetData(".appVPath", virtualDir);
			ad.SetData(".domainId", domainId);
			ad.SetData(".hostingVirtualPath", virtualDir);
			ad.SetData(".hostingInstallDir", aspDir);
			ObjectHandle oh = ad.CreateInstance(hostType.Module.Assembly.FullName, hostType.FullName);
			return oh.Unwrap();
		}
	}
}
