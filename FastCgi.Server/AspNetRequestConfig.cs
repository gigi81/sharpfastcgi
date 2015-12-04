using System;
using Grillisoft.FastCgi.AspNet;

namespace Grillisoft.FastCgi.Server
{
    public class AspNetRequestConfig : IAspNetRequestConfig
    {
        public string VirtualPath
        {
			get { return Config.VirtualPath; }
        }

        public string PhysicalPath
        {
            get { return Config.PhysicalPath; }
        }
    }
}
