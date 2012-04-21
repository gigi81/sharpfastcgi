using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;


namespace FastCgi.Protocol
{
	[Serializable]
	public class NameValuePairCollection : IEnumerable<NameValuePair>
	{
		private Dictionary<string, NameValuePair> _dictionary = new Dictionary<string, NameValuePair>();

		public NameValuePairCollection()
		{
		}

		public NameValuePairCollection(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
			{
				this.ReadCollection(reader);
			}
		}

		private void ReadCollection(BinaryReader reader)
		{
			while (true)
			{
				try
				{
					this.Add(new NameValuePair(reader));
				}
				catch (EndOfStreamException)
				{
					break; //exit loop
				}
			}
		}

		public string GetValue(string name)
		{
			NameValuePair ret;
			return _dictionary.TryGetValue(name, out ret) ? ret.Value : null;
		}

		public NameValuePair GetPair(string name)
		{
			NameValuePair ret;
			return _dictionary.TryGetValue(name, out ret) ? ret : null;
		}

		public void Add(NameValuePair pair)
		{
			_dictionary.Add(pair.Name, pair);
		}

		public void Add(IEnumerable<NameValuePair> collection)
		{
			foreach (NameValuePair item in collection)
				this.Add(item);
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		public bool ContainsKey(string name)
		{
			return _dictionary.ContainsKey(name);
		}

		#region IEnumerable implementation
		public IEnumerator<NameValuePair> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}
		#endregion
	}
}
