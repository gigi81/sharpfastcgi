using System;
using System.Text;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public struct BeginRequestMessageBody
	{
		public Role Role;
		public byte Flags;
		public byte[] Reserved;

        public BeginRequestMessageBody(Role role)
        {
            Role = role;
            Flags = 0x00;
            Reserved = new byte[5];
        }

        public BeginRequestMessageBody(ByteArray body)
        {
            Role = (Role)Utils.ReadUint16(body, 0);
            Flags = body[2];
            Reserved = body.SubArray(3).ToArray();
        }

        /// <summary>
        /// If false, the application closes the connection after responding to this request.
        /// If true, the application does not close the connection after responding to this request;
        /// the Web server retains responsibility for the connection.
        /// </summary>
        bool KeepConnection { get { return (this.Flags & 0x01) != 0; } }
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
