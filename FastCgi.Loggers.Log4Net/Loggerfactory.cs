using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grillisoft.FastCgi.Loggers.Log4Net
{
	[Serializable]
    public class LoggerFactory : Grillisoft.FastCgi.ILoggerFactory
    {
        public Grillisoft.FastCgi.ILogger Create(Type type)
        {
            return new Logger(log4net.LogManager.GetLogger(type));
        }

        public Grillisoft.FastCgi.ILogger Create(string name)
        {
            return new Logger(log4net.LogManager.GetLogger(name));
        }
    }
}
