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
using System.IO;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public class NameValuePair
	{
		public string Name { get; set; }

		public string Value { get; set; }

		public NameValuePair()
		{
		}

		public NameValuePair(BinaryReader reader)
		{
			int nameLength = GetLength(reader);
			int valueLength = GetLength(reader);

			this.Name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));
			this.Value = Encoding.UTF8.GetString(reader.ReadBytes(valueLength));
		}

		public void Encode(BinaryWriter writer)
		{
			byte[] name = Encoding.UTF8.GetBytes(this.Name);
			byte[] value = Encoding.UTF8.GetBytes(this.Value);

			writer.Write(SetLength(name.Length));
			writer.Write(SetLength(value.Length));
			writer.Write(name);
			writer.Write(value);
		}

		public static int GetLength(BinaryReader reader)
		{
			byte length = reader.ReadByte();
			if ((length & 0x80) == 0)
				return length;

            return ((length & 0x7F) << 24) +
                   (reader.ReadByte() << 16) +
			       (reader.ReadByte() << 8) +
			       (reader.ReadByte());
		}

		public static byte[] SetLength(int length)
		{
			if (length <= 0x7F)
				return new[] {(byte) length};

			return new[] {
				(byte)((length >> 24) & 0x7F),
				(byte)(length >> 16 & 0xFF),
				(byte)(length >> 8 & 0xFF),
				(byte)(length & 0xFF)
			};
		}
	}
}
