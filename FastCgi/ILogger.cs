using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi
{
    public enum LogLevel
    {
        Critical,
        Error,
        Warning,
        Info,
        Verbose
    }

    public interface ILogger
    {
        void Log(LogLevel level, string format, params object[] args);

        void Log(LogLevel level, Exception ex, string format, params object[] args);
    }
}
