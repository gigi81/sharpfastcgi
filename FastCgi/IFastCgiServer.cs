using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi
{
    public interface IFastCgiServer
    {
        void Start();

        void Stop();
    }
}
