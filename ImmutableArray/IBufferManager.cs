using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grillisoft.ImmutableArray
{
    public interface IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        IEnumerable<T[]> Allocate(int length);

        void Free(T[] data);
    }
}
