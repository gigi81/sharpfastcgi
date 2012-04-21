using System;
using System.Collections.Generic;
using System.IO;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	public class OutputStream : Stream
	{
		public event EventHandler<FlushEventArgs> Flushing;

		private ByteArray _array = ByteArray.Empty;

		public OutputStream()
		{
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
			if (_array.Count <= 0)
				return; //nothing to flush

			if (this.Flushing != null)
				this.Flushing(this, new FlushEventArgs(_array));

			_array = ByteArray.Empty;
		}

		public override long Length
		{
			get { return _array.Count; }
		}

		public override long Position { get; /* TODO: add setter checks */ set; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_array += new ByteArray(buffer, count, offset);
		}
	}

	public class FlushEventArgs : EventArgs
	{
		public FlushEventArgs(ByteArray data)
		{
			this.Data = data;
		}

		public ByteArray Data { get; private set; }
	}
}
