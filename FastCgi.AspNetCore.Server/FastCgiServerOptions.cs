using System;
using System.Collections.Generic;
using System.Text;

namespace Grillisoft.FastCgi.AspNetCore.Server
{
    public class FastCgiServerOptions
    {
        /// <summary>
        /// Enables the Listen options callback to resolve and use services registered by the application during startup.
        /// Typically initialized by UseKestrel()"/>.
        /// </summary>
        public IServiceProvider ApplicationServices { get; set; }
    }
}
