using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	/// <summary>
	/// A protocol layer that receives data from a lower layer
	/// </summary>
    public interface IUpperLayer
    {
        /// <summary>
        /// Receives data from the lower layer
        /// </summary>
        /// <param name="data">Received data</param>
        void Receive(ByteArray data);
    }

	/// <summary>
	/// A protocol layer that sends data received from an upper layer
	/// </summary>
    public interface ILowerLayer
    {
        /// <summary>
        /// Receives data from the upper layer
        /// </summary>
        /// <param name="data">Data to be sent</param>
        void Send(ByteArray data);
    }

	/// <summary>
	/// A protocol layer that both sends and recives data
	/// </summary>
    public interface ILayer : IUpperLayer, ILowerLayer
    {
    }
}
