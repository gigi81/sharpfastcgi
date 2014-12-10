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
    internal class ImmutableArrayEnumerator<T> : IEnumerator<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        private readonly List<ImmutableArrayInternal<T>> _arrays = null;
        private int _arrayIndex = 0;
        private int _index = -1;

        public ImmutableArrayEnumerator(List<ImmutableArrayInternal<T>> arrays)
        {
            _arrays = arrays;
        }

        #region IEnumerator<T> Membri di

        public T Current
        {
            get { return _arrays[_arrayIndex][_index]; }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Membri di

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            _index++;

            if (_index < _arrays[_arrayIndex].Length)
                return true;

            _arrayIndex++;
            _index = 0;
            
            return _arrayIndex < _arrays.Count && _index < _arrays[_arrayIndex].Length;
        }

        public void Reset()
        {
            _index = -1;
            _arrayIndex = 0;
        }
        #endregion
    }
}
