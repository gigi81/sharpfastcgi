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
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public class Message
	{
		public static readonly byte[] Padding = new byte[Consts.ChunkSize]; 

		public Message(ByteArray array)
		{
			this.Header = new MessageHeader(array);
			this.Body = array.SubArray(MessageHeader.HeaderSize, this.Header.ContentLength);
		}

		public Message(byte[] data)
			: this(new ByteArray(data))
		{
		}

		protected Message(MessageHeader header)
			: this(header, ByteArray.Empty)
		{
		}

		public Message(MessageType messageType, ushort requestId, byte[] body)
			: this(messageType, requestId, new ByteArray(body))
		{
		}

		public Message(MessageType messageType, ushort requestId, ByteArray body)
			: this(new MessageHeader(messageType, requestId, (ushort)body.Count), body)
		{
		}

		public Message(MessageHeader header, ByteArray body)
		{
			if (header.ContentLength != body.Count)
				throw new InvalidOperationException("Header ContentLength must be equals to body length");

			this.Header = header;
			this.Body = body;
		}

		public MessageHeader Header { get; protected set; }

		public ByteArray Body { get; protected set; }

		public int CopyTo(byte[] dest)
		{
			if (dest.Length < this.Header.MessageSize)
				throw new ArgumentException("dest");

			this.Header.CopyTo(dest);
			this.Body.CopyTo(dest, MessageHeader.HeaderSize);
			Array.Copy(Padding, 0, dest, MessageHeader.HeaderSize + this.Body.Count, this.Header.PaddingLength);

			return this.Header.MessageSize;
		}

		public ByteArray ToByteArray()
		{
			return new ByteArray(this.Header.ToArray()) + this.Body + new ByteArray(Padding, this.Header.PaddingLength);
		}
	}
}
