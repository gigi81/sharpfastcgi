#region License
//*****************************************************************************/
// Copyright (c) 2012 Luigi Grilli
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//*****************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.FastCgi.Protocol
{
	/// <summary>
	/// A <see cref="FastCgiChannel"/> that can handle only one request at a time
	/// </summary>
	/// <typeparam name="TRequest">Request type handled by the channel</typeparam>
	public abstract class SimpleFastCgiChannel : FastCgiChannel
	{
		public SimpleFastCgiChannel(ILowerLayer layer, ILoggerFactory loggerFactory)
            : base(layer, new Repositories.SingleRequestRepository(), loggerFactory)
		{
			this.Properties = new Protocol.ChannelProperties()
			{
				MaximumConnections = 1,
				MaximumRequests = 1,
				SupportMultiplexedConnection = false
			};
		}
	}
}
