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
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public struct BeginRequestMessageBody
	{
		public FastCgiRoles Role;
		public byte Flags;
		public byte[] Reserved;

        public BeginRequestMessageBody(FastCgiRoles role)
        {
            Role = role;
            Flags = 0x00;
            Reserved = new byte[5];
        }

        public BeginRequestMessageBody(ByteArray body)
        {
            Role = (FastCgiRoles)Utils.ReadUint16(body, 0);
            Flags = body[2];
            Reserved = body.ToArray(3, body.Count - 3);
        }

        /// <summary>
        /// If false, the application closes the connection after responding to this request.
        /// If true, the application does not close the connection after responding to this request;
        /// the Web server retains responsibility for the connection.
        /// </summary>
        public bool KeepConnection { get { return (this.Flags & 0x01) != 0; } }
	}

	public struct EndRequestMessageBody
	{
        /// <summary>
        /// Application Status
        /// </summary>
		public int ApplicationStatus;
		/// <summary>
        /// Protocol Status
		/// </summary>
		public ProtocolStatus ProtocolStatus;
		/// <summary>
        /// Reseved for future use and body padding
		/// </summary>
		public byte[] Reserved;

        public EndRequestMessageBody(int appStatus, ProtocolStatus protocolStatus)
        {
            ApplicationStatus = appStatus;
            ProtocolStatus = protocolStatus;
            Reserved = new byte[3];
        }

        public void CopyTo(byte[] dest)
        {
            this.CopyTo(dest, 0);
        }

        public void CopyTo(byte[] body, int startIndex)
        {
            body[startIndex + 0] = (byte) ((this.ApplicationStatus >> 24) & 0xFF);
            body[startIndex + 1] = (byte) ((this.ApplicationStatus >> 16) & 0xFF);
            body[startIndex + 2] = (byte) ((this.ApplicationStatus >> 8) & 0xFF);
            body[startIndex + 3] = (byte) ((this.ApplicationStatus) & 0xFF);
            body[startIndex + 4] = (byte) this.ProtocolStatus;
            body[startIndex + 5] = this.Reserved[0];
            body[startIndex + 6] = this.Reserved[1];
            body[startIndex + 7] = this.Reserved[2];
        }

        public byte[] ToArray()
        {
            var ret = new byte[Consts.ChunkSize];
            this.CopyTo(ret);
            return ret;
        }
	}
}
