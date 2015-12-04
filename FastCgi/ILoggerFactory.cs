using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi
{
    public interface ILoggerFactory
    {
        ILogger Create(Type type);

        ILogger Create(string name);
    }
}
