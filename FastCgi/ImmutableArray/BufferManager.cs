using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastCgi.ImmutableArray
{
    public class BufferManager<T>
    {
        public const int DefaultSize = 4096;

        /// <summary>
        /// Contains the list of the buffers in use
        /// </summary>
        private readonly HashSet<T[]> _buffers = new HashSet<T[]>();

        /// <summary>
        /// Contains the list of the buffers NOT in use
        /// </summary>
        private readonly Stack<T[]> _freeBuffers = new Stack<T[]>();

        /// <summary>
        /// Oject used to syncronise access to the BufferManager
        /// </summary>
        private readonly object _sync = new object();

        private int _count = 0;

        public void Init(int count)
        {
            lock (_sync)
            {
                while (count > 0)
                {
                    _freeBuffers.Push(this.CreateMutableBuffer());
                    count--;
                }
            }
        }

        /// <summary>
        /// The total number of <see cref="T"/> elements managed
        /// </summary>
        public long Size
        {
            get { return DefaultSize*_count; }
        }

        /// <summary>
        /// The total number of <see cref="T"/> elements free
        /// </summary>
        public long FreeSize
        {
            get
            {
                lock (_sync)
                {
                    return _freeBuffers.Count*DefaultSize;
                }
            }
        }

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public IEnumerable<T[]> Allocate(int size)
        {
            while (size > 0)
            {
                var ret = this.Allocate();
                size -= ret.Length;
                yield return ret;
            }
        }

        public T[] Allocate()
        {
            lock (_sync)
            {
                return GetFreeBuffer() ?? AllocateInternal();
            }
        }

        public void Free(T[] data)
        {
            lock (_sync)
            {
                if (!_buffers.Contains(data))
                    throw new Exception("Buffer specified was already freed or is not managed");

                _freeBuffers.Push(data);
                _buffers.Remove(data);
            }
        }

        private T[] AllocateInternal()
        {
            var ret = this.CreateMutableBuffer();
            _buffers.Add(ret);
            return ret;
        }

        private T[] GetFreeBuffer()
        {
            var ret = _freeBuffers.Count <= 0 ? null : _freeBuffers.Pop();
            if (ret != null)
                _buffers.Add(ret);

            return ret;
        }

        private T[] CreateMutableBuffer()
        {
            var ret = new T[DefaultSize];
            _count++;
            return ret;
        }
    }
}
