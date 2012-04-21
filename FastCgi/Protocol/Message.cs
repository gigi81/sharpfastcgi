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
