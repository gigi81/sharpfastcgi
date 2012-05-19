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
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Runtime.Remoting;
using System.Globalization;

namespace FastCgi.AspNet
{
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
