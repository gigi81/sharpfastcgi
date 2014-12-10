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

namespace FastCgi.Protocol
{
	public class ChannelProperties
	{
		public const string MaxConns    = "FCGI_MAX_CONNS";
		public const string MaxReqs     	= "FCGI_MAX_REQS";
		public const string MpxsConns   = "FCGI_MPXS_CONNS";

		/// <summary>
		/// The maximum number of concurrent transport connections this application will accept, e.g. 1 or 10. 
		/// </summary>
		public int MaximumConnections { get; set; }

		/// <summary>
		/// The maximum number of concurrent requests this application will accept, e.g. 1 or 50
		/// </summary>
		public int MaximumRequests { get; set; }

		/// <summary>
		/// Specify if this application does multiplex connections (i.e. handle concurrent requests over each connection)
		/// </summary>
		public bool SupportMultiplexedConnection { get; set; }

		/// <summary>
		/// Sets the values of the parameters specified in the collection
		/// </summary>
		/// <param name="collection">Collection containg the parametes to set</param>
		public void GetValues(NameValuePairCollection collection)
		{
			if (collection.ContainsKey(MaxConns))
				collection.GetPair(MaxConns).Value = this.MaximumConnections.ToString();

			if (collection.ContainsKey(MaxReqs))
				collection.GetPair(MaxReqs).Value = this.MaximumRequests.ToString();

			if (collection.ContainsKey(MpxsConns))
				collection.GetPair(MpxsConns).Value = this.SupportMultiplexedConnection ? "1" : "0";
		}
	}
}
