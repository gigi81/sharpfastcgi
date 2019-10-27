using System;

namespace Grillisoft.ImmutableArray
{
    public static class BufferManagerFactory<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        private static IBufferManager<T> _manager;

        public static IBufferManager<T> Instance
        {
            get => _manager ?? (_manager = new BufferManager<T>());
            set => _manager = value;
        }
    }
}
