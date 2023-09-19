using System;
using System.Collections;
using System.Collections.Generic;

namespace Sunshower
{
    public struct ArrayPool<T> : IDisposable, IReadOnlyCollection<T>
    {
        private readonly T[] _array;
        private bool _isDisposed;

        public Span<T> Span => _array;
        public T[] Array => _array;

        public int Count => _array.Length;

        public ref T this[int index] => ref _array[index];

        public ArrayPool(int minimumLength)
        {
            _array = System.Buffers.ArrayPool<T>.Shared.Rent(minimumLength);
            _isDisposed = false;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                System.Buffers.ArrayPool<T>.Shared.Return(_array);
                _isDisposed = true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
