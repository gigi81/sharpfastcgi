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
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public struct MessageHeader
	{
		/// <summary>
		/// Record header size in bytes
		/// </summary>
		public const ushort HeaderSize = Consts.ChunkSize;

		/// <summary>
		/// FastCGI version number
		/// </summary>
		public byte Version;

		/// <summary>
		/// Record type
		/// </summary>
		public MessageType MessageType;

		/// <summary>
		/// Request ID
		/// </summary>
		public ushort RequestId;

		/// <summary>
		/// Content length
		/// </summary>
		public ushort ContentLength;

		/// <summary>
		/// Length of record padding
		/// </summary>
		public byte PaddingLength;

		/// <summary>
		/// Reseved for future use and header padding
		/// </summary>
		public byte Reserved;

		public MessageHeader(MessageType messageType, ushort requestId, ushort contentLength)
		{
			Version = Consts.Version;
			MessageType = messageType;
			RequestId = requestId;
			ContentLength = contentLength;
            PaddingLength = CalculatePadding(contentLength);
			Reserved = 0x00;
		}

        public MessageHeader(ByteArray array)
            : this(array.ToArray(0, HeaderSize))
        {
        }

		public MessageHeader(byte[] header)
			: this(header, 0)
		{
		}

		public MessageHeader(byte[] header, int startIndex)
		{
			Version 	  = header[startIndex + 0];
			MessageType	  = (MessageType)header[startIndex + 1];
			RequestId 	  = Utils.ReadUint16(header, startIndex + 2);
            ContentLength = Utils.ReadUint16(header, startIndex + 4);
			PaddingLength = header[startIndex + 6];
			Reserved 	  = header[startIndex + 7];
		}

		public RecordType RecordType
		{
			get
			{
				return (this.RequestId == 0) ? RecordType.Management : RecordType.Application;
			}
		}
		
        /// <summary>
        /// Size of the entire message complete with header, content and padding
        /// </summary>
        public int MessageSize
        {
            get
            {
                return HeaderSize + this.ContentLength + this.PaddingLength;
            }
        }

		public void CopyTo(byte[] dest)
		{
			this.CopyTo(dest, 0);
		}

		public void CopyTo(byte[] header, int startIndex)
		{
			header[startIndex + 0] = Version;
			header[startIndex + 1] = (byte) MessageType;
			header[startIndex + 2] = (byte) ((RequestId >> 8) & 0xFF);
			header[startIndex + 3] = (byte) (RequestId & 0xFF);
			header[startIndex + 4] = (byte) ((ContentLength >> 8) & 0xFF);
			header[startIndex + 5] = (byte) (ContentLength & 0xFF);
			header[startIndex + 6] = PaddingLength;
			header[startIndex + 7] = Reserved;
		}

		public byte[] ToArray()
		{
			var ret = new byte[HeaderSize];
			this.CopyTo(ret);
			return ret;
		}

        private static byte CalculatePadding(ushort contentLength)
        {
            if (contentLength % Consts.ChunkSize == 0)
                return 0;

            return (byte)(Consts.ChunkSize - (contentLength % Consts.ChunkSize));
        }
	}
}
