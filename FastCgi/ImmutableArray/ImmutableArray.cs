#region License
//*****************************************************************************/
// Copyright (c) 2010 - 2012 Luigi Grilli
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastCgi.ImmutableArray
{
	/// <summary>
	/// Immutable array
	/// </summary>
	/// <typeparam name="T">Elements type of the array</typeparam>
	public class ImmutableArray<T> : ICloneable, ICollection, IEnumerable<T> where T : struct, IComparable, IEquatable<T>, IConvertible
	{
		/// <summary>
		/// Empty array
		/// </summary>
		public static readonly ImmutableArray<T> Empty = new ImmutableArray<T>(new T[0]);

		private List<ImmutableArrayInternal<T>> _arrays = new List<ImmutableArrayInternal<T>>();
		private List<int> _offsets = new List<int>();
		private int _length = 0;

		#region Constructors
		/// <summary>
		/// Creates an immutable array cloning the specified array
		/// </summary>
		/// <param name="data">Array to clone</param>
		public ImmutableArray(T[] data)
			: this(data, data.Length)
		{
		}

		/// <summary>
		/// Creates an immutable array cloning the specified array
		/// </summary>
		/// <param name="data">Array to clone</param>
		/// <param name="length">Length of the data</param>
		public ImmutableArray(T[] data, int length)
			: this(data, length, 0)
		{
		}

		/// <summary>
		/// Creates an immutable array cloning the specified array
		/// </summary>
		/// <param name="data">Array to clone</param>
		/// <param name="length">Length of the data</param>
		/// <param name="sourceIndex">Index of the cloned array where to start cloning</param>
		public ImmutableArray(T[] data, int length, int sourceIndex)
		{
			this.Add(new ImmutableArrayInternal<T>(sourceIndex, data, length));
		}

		/// <summary>
		/// Creates an immutable array cloning the specifie array
		/// </summary>
		/// <param name="arrays">Array to be cloned</param>
		public ImmutableArray(ImmutableArray<T> array)
		{
			this.Add(array);
		}

		/// <summary>
		/// Creates an immutable array cloning the specified arrays
		/// </summary>
		/// <param name="arrays">Arrays to be cloned</param>
		public ImmutableArray(ImmutableArray<T>[] arrays)
		{
			foreach (ImmutableArray<T> byteArray in arrays)
				this.Add(byteArray);
		}

		public ImmutableArray(ICollection<T> array)
		{
			this.Add(new ImmutableArrayInternal<T>(array));
		}

		/// <summary>
		/// Creates an immutable array concatenating the specified array of immutable arrays
		/// </summary>
		/// <param name="arrays">Immutable arrays to concatenate</param>
		private ImmutableArray(IEnumerable<ImmutableArrayInternal<T>> arrays)
		{
			foreach (ImmutableArrayInternal<T> array in arrays)
				this.Add(array);
		}
		#endregion

		#region Public methods/properties
		/// <summary>
		/// Length of the array
		/// </summary>
		public int Count
		{
			get { return _length; }
		}

		/// <summary>
		/// Gets the element with the specified index
		/// </summary>
		/// <param name="index">Index of the element (0-based)</param>
		/// <returns>Element value</returns>
		public T this[int index]
		{
			get
			{
				int arrayIndex = this.GetArrayIndexAt(ref index);
				return _arrays[arrayIndex][index];
			}
		}

		/// <summary>
		/// Gets a portion of the array starting from the specified index to the end
		/// </summary>
		/// <param name="sourceIndex">Index to start from</param>
		/// <returns>An immutable array that is a portion of this array</returns>
		public ImmutableArray<T> SubArray(int sourceIndex)
		{
			return this.SubArray(sourceIndex, _length - sourceIndex);
		}

		/// <summary>
		/// Gets a portion of the array starting from the specified index for the specified length
		/// </summary>
		/// <param name="sourceIndex">Index to start from</param>
		/// <param name="length">Length of the portion</param>
		/// <returns>An immutable array that is a portion of this array</returns>
		public ImmutableArray<T> SubArray(int sourceIndex, int length)
		{
			if ((_length == sourceIndex) && (length == 0))
				return Empty;

            if ((sourceIndex + length) > _length)
                throw new ArgumentOutOfRangeException("The sourceIndex and length specified overcome the array length");

			var ret = new List<ImmutableArrayInternal<T>>();
			var i = this.GetArrayIndexAt(ref sourceIndex);

			for (; length > 0; i++)
			{
                var array = _arrays[i].SubArrayInternal(sourceIndex, _arrays[i].Length > length ? length : _arrays[i].Length);

				ret.Add(array);
				length -= array.Length;
                sourceIndex = 0;
			}

			return new ImmutableArray<T>(ret);
		}

		/// <summary>
		/// Concatenates this array with the specified immutable array and returns a new immutable array
		/// </summary>
		/// <param name="array">Array to concatenate with this one</param>
		/// <returns>An immutable array</returns>
		public ImmutableArray<T> Concat(ImmutableArray<T> array)
		{
			return new ImmutableArray<T>(new ImmutableArray<T>[] { this, array });
		}

		/// <summary>
		/// Concatenates this array with the specified arrays and returns a new immutable array
		/// </summary>
		/// <param name="array">Arrays to concatenate with this one</param>
		/// <returns>An immutable array</returns>
		public ImmutableArray<T> Concat(T[] array)
		{
			return new ImmutableArray<T>(new ImmutableArray<T>[] { this, new ImmutableArray<T>(array) });
		}

		/// <summary>
		/// Concatenates this array with the specified immutable arrays and returns a new immutable array
		/// </summary>
		/// <param name="array">Arrays to concatenate with this one</param>
		/// <returns>An immutable array</returns>
		public ImmutableArray<T> Concat(ImmutableArray<T>[] array)
		{
			return new ImmutableArray<T>(new ImmutableArray<T>[] { this, new ImmutableArray<T>(array) });
		}

		/// <summary>
		/// Concatenates the specified immutable array with the specified arrays and returns a new immutable array
		/// </summary>
		/// <param name="array1">Immutable array to concatenate</param>
		/// <param name="array2">Arrays to concatenate with the first one</param>
		/// <returns>An immutable array</returns>
		public static ImmutableArray<T> operator +(ImmutableArray<T> array1, T[] array2)
		{
			return array1.Concat(array2);
		}

		/// <summary>
		/// Concatenates the specified immutable array with the specified immutable arrays and returns a new immutable array
		/// </summary>
		/// <param name="array1">Immutable array to concatenate</param>
		/// <param name="array2">Arrays to concatenate with the first one</param>
		/// <returns>An immutable array</returns>
		public static ImmutableArray<T> operator +(ImmutableArray<T> array1, ImmutableArray<T> array2)
		{
			return array1.Concat(array2);
		}

		public static bool operator ==(ImmutableArray<T> array1, ImmutableArray<T> array2)
		{
			if ((object)array1 == null || (object)array2 == null)
				return object.ReferenceEquals(array1, array2);

			return array1.Equals(array2);
		}

		public static bool operator !=(ImmutableArray<T> array1, ImmutableArray<T> array2)
		{
			if ((object)array1 == null || (object)array2 == null)
				return !object.ReferenceEquals(array1, array2);

			return !array1.Equals(array2);
		}

		public static bool operator ==(ImmutableArray<T> array1, T[] array2)
		{
			return array1.Equals(array2);
		}

		public static bool operator !=(ImmutableArray<T> array1, T[] array2)
		{
			return array1.Equals(array2);
		}

		/// <summary>
		/// Creates a copy of this immutable array to a mutable array
		/// </summary>
		/// <returns>A mutable array containing a copy of this array</returns>
		public T[] ToArray()
		{
			T[] ret = new T[_length];
			this.CopyTo(ret, 0);
			return ret;
		}
		#endregion

		#region Private methods/properties
		private void Add(ImmutableArray<T> array)
		{
			foreach (ImmutableArrayInternal<T> item in array._arrays)
				this.Add(item);
		}

		private void Add(ImmutableArrayInternal<T> array)
		{
			if (array.Length <= 0)
				return;

			_arrays.Add(array);
			_offsets.Add(_length + array.Length);
			_length += array.Length;
		}

		/// <summary>
		/// Finds the index of the internal array containing the i-th element in the immutable array
		/// </summary>
		/// <param name="index">IN: the i-th element in the immutable array, OUT: the i-th element in the internal array</param>
		/// <returns>Index of the internal array containing the data</returns>
		/// <remarks>
		/// This is the second version of the method.
		/// It uses an offset array to speed up the search of the internal array index.
		/// This is designed for performance more than code readability
		/// </remarks>
		private int GetArrayIndexAt(ref int index)
		{
			if ((index < 0) || (index >= _length))
				throw new IndexOutOfRangeException(String.Format("Index {0} out of array bounds", index));

			//we exclude that the data is in the first internal array
			if (index < _offsets[0])
				return 0;

			int min = 1;
			int max = _offsets.Count - 1;

			while(index >= _offsets[min])
			{
				int middle = min + ((max - min) >> 1);
				if (middle == min) //when (max - min) < 2 this can happen
					middle++;

				if (index < _offsets[middle])
				{
					max = middle;
					min++; //we already know that min is not valid so why waste a loop cicle?
				}
				else //index >= _offsets[middle]
				{
					min = middle;
				}
			}

			index -= _offsets[min - 1];
			return min;
		}

		private int GetArrayIndexAt_v1(ref int index)
		{
			if ((index < 0) || (index >= _length))
				throw new IndexOutOfRangeException(String.Format("Index {0} out of array bounds", index));
		
			for(int i=0; i<_arrays.Count; i++)
			{
				if (index < _arrays[i].Length)
					return i;

				index -= _arrays[i].Length;
			}
		
			return -1;
		}
		#endregion

		#region Object overrides
		/// <summary>
		/// Compares for equality this array with the specified object
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if the two objects matches</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("Cannot compare object with a null instance");

			//check if is the same reference
			if (obj == (object)this)
				return true;

			if (typeof(T[]).IsInstanceOfType(obj))
				return this.EqualsTo((T[])obj);

			if (typeof(ImmutableArray<T>).IsInstanceOfType(obj))
				return this.EqualsTo((ImmutableArray<T>)obj);

			return base.Equals(obj);
		}

		/// <summary>
		/// Calculate the hash code of the array
		/// </summary>
		/// <returns>The array hash code</returns>
		public override int GetHashCode()
		{
			int step = _length / ImmutableArrayInternal<T>.HASH_MAX_ELEMENTS;
			if (step <= 0)
				step = 1;

			int hash = 0;

			for (int i = 0; i < _length; i += step)
				hash += this[i].GetHashCode();

			return hash;
		}

		/// <summary>
		/// Creates a string with all the elements of the array
		/// </summary>
		/// <returns>String representing the array</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			foreach (ImmutableArrayInternal<T> array in _arrays)
			{
				if (builder.Length > 0)
					builder.Append(ImmutableArrayInternal<T>.SEPARATOR);

				builder.Append(array.ToString());
			}

			return builder.ToString();
		}
		#endregion

		#region EqualsTo methods
		protected bool EqualsTo(IEnumerable<T> array)
		{
			IEnumerator<T> enum1 = this.GetEnumerator();
			IEnumerator<T> enum2 = array.GetEnumerator();

			while (enum1.MoveNext() && enum2.MoveNext())
				if (!enum1.Current.Equals(enum2.Current))
					return false;

			return true;
		}

		/// <summary>
		/// Compare this immutable array with the specified mutable array
		/// </summary>
		/// <param name="array">Array to compare</param>
		/// <returns>True if all the elements of the two array equals</returns>
		protected bool EqualsTo(T[] array)
		{
			if (_length != array.Length)
				return false;

			for (int i = 0; i < _length; i++)
				if (!this[i].Equals(array[i]))
					return false;

			return true;
		}

		/// <summary>
		/// Compare this immutable array with the specified immutable array
		/// </summary>
		/// <param name="array">Array to compare</param>
		/// <returns>True if all the elements of the two array equals</returns>
		protected bool EqualsTo(ImmutableArray<T> array)
		{
			if (_length != array.Count)
				return false;

			//fast compare
			if (this.GetHashCode() != array.GetHashCode())
				return false;

			//slow compare
			for (int i = 0; i < _length; i++)
				if (!this[i].Equals(array[i]))
					return false;

			return true;
		}
		#endregion

		#region Members of ICloneable
		/// <summary>
		/// Clone this array. As long as this is an immutable object it returns the reference of this object
		/// </summary>
		/// <returns>The reference of this object</returns>
		public object Clone()
		{
			return this;
		}
		#endregion

        #region Members of IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
		{
			return new ImmutableArrayEnumerator<T>(_arrays);
		}

		#endregion

        #region Members of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
		{
			return new ImmutableArrayEnumerator<T>(_arrays);
		}

		#endregion

		#region Members of ICollection
		/// <summary>
		/// Copy all the data from this immutable array to the mutable array specified at the offset specified
		/// </summary>
		/// <param name="array">Destination array to copy data to</param>
		/// <param name="index">Offset of the destination array where to start copying data</param>
		public void CopyTo(Array array, int index)
		{
			this.CopyTo(array, index, _length);
		}

		/// <summary>
		/// Copy the number of elements specified from this immutable array to the mutable array specified at the offset specified
		/// </summary>
		/// <param name="array">Destination array to copy data to</param>
		/// <param name="index">Offset of the destination array where to start copying data</param>
		/// <param name="length">Number of elements to copy</param>
		public void CopyTo(Array array, int index, int length)
		{
			foreach (var source in _arrays)
			{
				if (length <= 0)
					break;

				int part = Math.Min(source.Length, length);

				source.CopyTo(array, index, part);
				index += part;
				length -= part;
			}
		}

		public bool IsSynchronized
		{
			get { return true; }
		}

		public object SyncRoot
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	}
}
