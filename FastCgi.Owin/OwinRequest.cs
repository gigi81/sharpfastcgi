using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Grillisoft.FastCgi.Protocol;
using Microsoft.Owin;

namespace Grillisoft.FastCgi.Owin
{
    public class OwinRequest : Request
    {
        private readonly OwinMiddleware pipeline;
        private readonly OwinContext context = new OwinContext();
        private List<Tuple<Action<object>, object>> onSendHeadersCallbacks = new List<Tuple<Action<object>, object>>();
        private readonly MemoryStream responseBody = new MemoryStream();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public OwinRequest(ushort id, BeginRequestMessageBody body, OwinMiddleware pipeline) : base(id, body)
        {
            this.pipeline = pipeline;
            context.Set<Action<Action<object>, object>>("server.OnSendingHeaders", RegisterOnSendingHeadersCallback);
        }

        public override void Abort()
        {
            cts.Cancel();
        }

        public override void Execute()
        {
            try
            {
                SetOwinContextValues();

                using (var task = pipeline.Invoke(context))
                {
                    task.Wait();
                }

                SendHeaders();
                responseBody.Position = 0;
                responseBody.CopyTo(this.OutputStream);
            }
            // TODO: Catch exceptions and log
            finally
            {
                this.End();
                this.responseBody.Dispose();
                this.cts.Dispose();
            }
        }

        private void SetOwinContextValues()
        {
            context.Environment.Add(Constants.OwinVersion);
            context.Request.CallCancelled = cts.Token;
            context.Request.Body = this.InputStream.Length == 0 ? Stream.Null : this.InputStream;
            context.Response.Body = responseBody;
            var headers = context.Request.Headers;

            foreach (var nvp in this.Parameters)
            {
                string uName = nvp.Name.ToUpperInvariant();
                switch (uName)
                {
                    case "SERVER_PROTOCOL":
                        context.Request.Protocol = nvp.Value;
                        continue;
                    case "REQUEST_METHOD":
                        context.Request.Method = nvp.Value.ToUpperInvariant();
                        continue;
                    case "QUERY_STRING":
                        context.Request.QueryString = new QueryString(nvp.Value);
                        continue;
                    case "HTTPS":
                        context.Request.Scheme = nvp.Value.Equals("on", StringComparison.OrdinalIgnoreCase) ? "https" : "http";
                        continue;
                    case "PATH_INFO":
                        context.Request.PathBase = new PathString(string.Empty);
                        context.Request.Path = new PathString(nvp.Value);
                        continue;
                }

                if (uName.StartsWith("HTTP_"))
                {
                    headers.Add(uName.Substring(5).Replace('_', '-'), new string[] { nvp.Value } );
                }
            }

            context.Response.Protocol = context.Request.Protocol;
        }

        private void SendHeaders()
        {
            foreach (var callbackTuple in onSendHeadersCallbacks)
            {
                callbackTuple.Item1(callbackTuple.Item2);
            }

            using (var writer = new StreamWriter(this.OutputStream, Encoding.ASCII, 1024, true))
            {
                writer.WriteLine($"Status: {context.Response.StatusCode} {context.Response.ReasonPhrase ?? Constants.ReasonPhrases[context.Response.StatusCode]}");

                foreach (var header in context.Response.Headers)
                {
                    writer.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                if (!context.Response.ContentLength.HasValue)
                {
                    writer.WriteLine($"Content-Length: {responseBody.Length}");
                }

                writer.WriteLine();
            }
        }

        private void RegisterOnSendingHeadersCallback(Action<object> callback, object state)
        {
            onSendHeadersCallbacks.Add(Tuple.Create(callback, state));
        }
    }
}
