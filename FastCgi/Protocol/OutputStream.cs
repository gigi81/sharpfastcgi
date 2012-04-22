using System;
using System.Collections.Generic;
using System.IO;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	/// <summary>
	/// Output stream to write data to
	/// </summary>
	public class OutputStream : Stream
	{
		private MemoryStream _chache = new MemoryStream(256);

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
			this.WriteCache();

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
			if (count <= 16)
			{
				_chache.Write(buffer, offset, count);
			}
			else
			{
				this.WriteCache();
				_array += new ByteArray(buffer, count, offset);
			}
		}

		private void WriteCache()
		{
			if (_chache.Length <= 0)
				return;

			_array += new ByteArray(_chache.ToArray());
			_chache.SetLength(0);
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
