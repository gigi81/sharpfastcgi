using Grillisoft.FastCgi.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grillisoft.FastCgi.AspNetCore.Server
{
    public class AspNetCoreRequest : Request
    {
        public AspNetCoreRequest(ushort id, BeginRequestMessageBody body)
            : base(id, body)
        {
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
