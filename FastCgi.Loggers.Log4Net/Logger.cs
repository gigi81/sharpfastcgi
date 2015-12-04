using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grillisoft.FastCgi.Loggers.Log4Net
{
    internal class Logger : Grillisoft.FastCgi.ILogger
    {
        private readonly log4net.ILog _logger;

        public Logger(log4net.ILog logger)
        {
            _logger = logger;
        }

        public void Log(Grillisoft.FastCgi.LogLevel level, string format, params object[] args)
        {
            switch (level)
            {
                case Grillisoft.FastCgi.LogLevel.Critical:
                    _logger.FatalFormat(format, args);
                    break;
                case Grillisoft.FastCgi.LogLevel.Error:
                    _logger.ErrorFormat(format, args);
                    break;
                case Grillisoft.FastCgi.LogLevel.Warning:
                    _logger.WarnFormat(format, args);
                    break;
                case Grillisoft.FastCgi.LogLevel.Info:
                    _logger.InfoFormat(format, args);
                    break;
                case Grillisoft.FastCgi.LogLevel.Verbose:
                    _logger.DebugFormat(format, args);
                    break;
                default:
                    break;
            }
        }

        public void Log(Grillisoft.FastCgi.LogLevel level, Exception ex, string format, params object[] args)
        {
            try
            {
                format = format ?? String.Empty;
                args = args ?? new object[0];

                switch (level)
                {
                    case Grillisoft.FastCgi.LogLevel.Critical:
                        _logger.Fatal(String.Format(format, args), ex);
                        break;
                    case Grillisoft.FastCgi.LogLevel.Error:
                        _logger.Error(String.Format(format, args), ex);
                        break;
                    case Grillisoft.FastCgi.LogLevel.Warning:
                        _logger.Warn(String.Format(format, args), ex);
                        break;
                    case Grillisoft.FastCgi.LogLevel.Info:
                        _logger.Info(String.Format(format, args), ex);
                        break;
                    case Grillisoft.FastCgi.LogLevel.Verbose:
                        _logger.Debug(String.Format(format, args), ex);
                        break;
                    default:
                        break;
                }
            }
            catch(FormatException formatException)
            {
                _logger.Error("Invalid log format", formatException);
            }
        }
    }
}
