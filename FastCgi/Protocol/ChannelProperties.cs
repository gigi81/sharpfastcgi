using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastCgi.Protocol
{
	public class ChannelProperties
	{
		public const string MaxConns 	= "FCGI_MAX_CONNS";
		public const string MaxReqs 	= "FCGI_MAX_REQS";
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
