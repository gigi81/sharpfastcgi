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
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.Protocol
{
	/// <summary>
	/// Input stream to read data from
	/// </summary>
    public class InputStream : Stream
    {
        private ByteArray _array;

        public InputStream()
            : this(ByteArray.Empty)
        {
        }

        public InputStream(ByteArray array)
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            _array = array;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return _array.Count; }
        }

        public override long Position { get; /* TODO: add setter checks */ set; }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Position == this.Length)
                throw new EndOfStreamException();

            int max = (int)(this.Length - this.Position);
            if (count > max)
                count = max;

            _array.SubArray((int)this.Position, count).CopyTo(buffer, offset);
            this.Position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;

                case SeekOrigin.Current:
                    this.Position += offset;
                    break;

                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;

                default:
                    break;
            }

            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(); 
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public ByteArray Data
        {
            get { return _array; }
        }

        public void Append(ByteArray array)
        {
            _array += array;
        }
    }
}
