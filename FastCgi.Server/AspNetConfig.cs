using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Grillisoft.FastCgi.AspNet;

namespace Grillisoft.FastCgi.Server
{
    public class AspNetRequestConfig : IAspNetRequestConfig
    {
        public const string DefaultVirtualPath = "/";
        public const string DefaultPhysicalPath = "Root";

        public string VirtualPath
        {
            get { return ConfigurationManager.AppSettings["VirtualPath"] ?? DefaultVirtualPath; }
        }

        public string PhysicalPath
        {
            get
            {
                return Path.GetFullPath(ConfigurationManager.AppSettings["PhysicalPath"] ?? DefaultPhysicalPath);
            }
        }
    }
}
