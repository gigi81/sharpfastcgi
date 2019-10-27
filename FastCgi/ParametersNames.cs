using System;

namespace Grillisoft.FastCgi
{
    public static class ParametersNames
    {
        public const string RequestMethod = "REQUEST_METHOD";
        public const string ServerProtocol = "SERVER_PROTOCOL";
        public const string ServerAddress = "SERVER_ADDR";
        public const string ServerPort = "SERVER_PORT";
        public const string QueryString = "QUERY_STRING";
        public const string RequestUri = "REQUEST_URI";
        public const string RemoteAddress = "REMOTE_ADDR";
        public const string RemotePort = "REMOTE_PORT";

        public static string ToParameterName(this string headerName)
        {
            if (String.IsNullOrEmpty(headerName))
                throw new ArgumentNullException(nameof(headerName));

            return "HTTP_" + headerName.ToUpper().Replace('-', '_');
        }

        public static string ToHeaderName(this string parameterName)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            if (!parameterName.StartsWith("HTTP_"))
                throw new ArgumentException("Invalid parameter {parameterName}, must start with HTTP_", nameof(parameterName));

            return parameterName.Substring(5).Replace('_', '-');
        }
    }
}
