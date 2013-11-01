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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;


namespace FastCgi.Protocol
{
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
		    var stream = reader.BaseStream;

            while (stream.Position < stream.Length /* check for stream EOF */)
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
