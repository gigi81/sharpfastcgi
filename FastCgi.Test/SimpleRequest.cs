using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Grillisoft.FastCgi.Protocol;
using System.Threading;

namespace Grillisoft.FastCgi.Test
{
    public class SimpleRequest : Request
    {
        public static int _requests = 0;

        public SimpleRequest(ushort id, BeginRequestMessageBody body)
            : base(id, body)
        {
        }

        public override void Execute()
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(this.GetResponseContent());
                var header = Encoding.UTF8.GetBytes(this.GetResponseHeaders(body.Length));

                this.OutputStream.Write(header, 0, header.Length);
                this.OutputStream.Write(body, 0, body.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this.ExitCode = -1;
            }
            finally
            {
                this.End();
            }
        }

        private string GetResponseHeaders(int contentLength)
        {
            var header = new StringBuilder();

            header.AppendFormat("Content-Length: {0}\r\n", contentLength);
            header.Append("Content-Type: text/html; charset=UTF-8\r\n");
            header.Append("\r\n");

            return header.ToString();
        }

        private string GetResponseContent()
        {
            var body = new StringBuilder();

            body.Append("<html><body><table>");
            foreach (var parameter in this.Parameters)
            {
                body.Append("<tr>");
                body.Append("<td>");
                body.Append(parameter.Name);
                body.Append("</td>");
                body.Append("<td>");
                body.Append(parameter.Value);
                body.Append("</td>");
                body.Append("</tr>");
            }

            body.Append("</table></body></html>");

            return body.ToString();
        }

        public override void Abort()
        {
            //TODO
        }
    }
}
