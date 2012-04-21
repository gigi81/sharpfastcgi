using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCgi.Tcp;

namespace FastCgi.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleServer server = new SimpleServer(9000);
            server.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
