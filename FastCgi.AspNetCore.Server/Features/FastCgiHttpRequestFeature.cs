using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Grillisoft.FastCgi.AspNetCore.Server.Features
{
    public class FastCgiHttpRequestFeature : IHttpRequestFeature
    {
        private readonly AspNetCoreRequest _request;

        public FastCgiHttpRequestFeature(AspNetCoreRequest request)
        {
            _request = request;
        }

        public string Protocol
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public string Scheme
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public string Method
        {
            get { return GetParameter(ParametersNames.RequestMethod); }
            set { SetParameter(ParametersNames.RequestMethod); }
        }

        public string PathBase
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public string Path
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public string QueryString
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public string RawTarget
        {
            get { return GetParameter(ParametersNames.ServerProtocol); }
            set { SetParameter(ParametersNames.ServerProtocol); }
        }

        public IHeaderDictionary Headers
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Stream Body
        {
            get => _request.InputStream;
            set => throw new NotImplementedException();
        }

        private string GetParameter(string parameter)
        {
            return _request.Parameters.GetValue(parameter);
        }

        private void SetParameter(string parameter)
        {
            //TODO
        }
    }
}
