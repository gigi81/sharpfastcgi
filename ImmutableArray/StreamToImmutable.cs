using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Grillisoft.ImmutableArray
{
    public class StreamToImmutable : Stream
    {
        private bool _disposed;

        private readonly List<byte[]> _buffers = new List<byte[]>();
        private int _length = 0;
        private int _position = 0;
        private int _size = 0;

        private int _currentIndex = 0;
        private int _currentOffset = 0;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set
            {
                if (_position > _size)
                    throw new ArgumentException("Cannot set position outside of the array");

                _position = (int)value;
                if (_position > _length)
                    _length = _position;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new InvalidOperationException("Cannot write to a disposed stream");

            this.Alloc(count);

            while(count > 0)
            {
                var dest = _buffers[_currentIndex];
                var max = Math.Min(dest.Length - _currentOffset, count);

                Array.Copy(buffer, offset, dest, _currentOffset, max);

                _currentOffset += max;
                if (_currentOffset >= buffer.Length)
                {
                    _currentIndex++;
                    _currentOffset = 0;
                }

                this.Position += max;
                count -= max;
            }
        }

        private void Alloc(int count)
        {
            var needed = count - (_size - _position);
            if (needed <= 0)
                return;

            needed = Math.Max(1024, needed); //we allocate at least 1024 bytes
            _buffers.AddRange(BufferManagerFactory<byte>.Instance.Allocate(needed));
            _size += needed;
        }

        public ImmutableArray<byte> ToImmutableArray()
        {
            _disposed = true;
            return new ImmutableArray<byte>(GetInternal());
        }

        private IEnumerable<ImmutableArrayInternal<byte>> GetInternal()
        {
            var left = _length;

            foreach(var buffer in _buffers)
            {
                var size = Math.Min(left, buffer.Length);
                yield return new ImmutableArrayInternal<byte>(buffer, size);

                left -= size;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            foreach (var buffer in _buffers)
                BufferManagerFactory<byte>.Instance.Free(buffer);
        }
    }
}
