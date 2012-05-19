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

			return length & 0x7F << 24 +
					 reader.ReadByte() << 16 +
					 reader.ReadByte() << 8 +
					 reader.ReadByte();
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
