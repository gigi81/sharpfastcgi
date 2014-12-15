using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using FastCgi.Protocol;

namespace FastCgi.Test
{
    public class SimpleRequest : Request
    {
        public SimpleRequest(ushort id, BeginRequestMessageBody body)
            : base(id, body)
        {
        }

        public override void Execute()
        {
            try
            {
                StringBuilder body = new StringBuilder();

                body.Append("<html><body><table>");
                foreach (NameValuePair param in this.Parameters)
                {
                    body.Append("<tr>");
                    body.Append("<td>");
                    body.Append(param.Name);
                    body.Append("</td>");
                    body.Append("<td>");
                    body.Append(param.Value);
                    body.Append("</td>");
                    body.Append("</tr>");
                }

                body.Append("</table></body></html>");

                StringBuilder header = new StringBuilder();
                header.AppendFormat("Content-Length: {0}\r\n", body.Length);
                header.Append("Content-Type: text/html; charset=UTF-8\r\n");
                header.Append("\r\n");

                byte[] data = Encoding.UTF8.GetBytes(header.ToString());
                this.OutputStream.Write(data, 0, data.Length);

                byte[] data2 = Encoding.UTF8.GetBytes(body.ToString());
                this.OutputStream.Write(data2, 0, data2.Length);
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

        public override void Abort()
        {
            //TODO
        }
    }
}
