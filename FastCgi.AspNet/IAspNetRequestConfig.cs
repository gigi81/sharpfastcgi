using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.AspNet
{
    public interface IAspNetRequestConfig
    {
        string VirtualPath { get; }

        string PhysicalPath { get; }
    }
}
