using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
    public interface IUpperLayer
    {
        /// <summary>
        /// Receives data from the lower layer
        /// </summary>
        /// <param name="data">Received data</param>
        void Receive(ByteArray data);
    }

    public interface ILowerLayer
    {
        /// <summary>
        /// Receives data from the upper layer
        /// </summary>
        /// <param name="data">Data to be sent</param>
        void Send(ByteArray data);
    }

    public interface ILayer : IUpperLayer, ILowerLayer
    {
    }
}
