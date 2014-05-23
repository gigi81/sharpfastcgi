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
                StringBuilder builder = new StringBuilder();
                
                builder.Append("Content-Type: text/html; charset=UTF-8\r\n\r\n");

                builder.Append("<html><body><table>");
                foreach (NameValuePair param in this.Parameters)
                {
                    builder.Append("<tr>");
                    builder.Append("<td>");
                    builder.Append(param.Name);
                    builder.Append("</td>");
                    builder.Append("<td>");
                    builder.Append(param.Value);
                    builder.Append("</td>");
                    builder.Append("</tr>");
                }

                builder.Append("</table></body></html>");

                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                this.OutputStream.Write(data, 0, data.Length);
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
