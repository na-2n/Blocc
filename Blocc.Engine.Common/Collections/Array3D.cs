using System;

namespace Blocc.Engine.Common
{
    public class Array3D<T>
    {
        private readonly Memory<T> _mem;
        private readonly int _width;
        private readonly int _depth;

        public Span<T> Span => _mem.Span;

        public T this[int x, int y, int z]
        {
            get => GetAt(x, y, z);
            set => SetAt(x, y, z, value);
        }

        public Array3D(int width, int height, int depth)
        {
            _mem = new T[width * depth * height];
            _width = width;
            _depth = depth;
        }

        private int GetArrayPos(int x, int y, int z)
            => y * (_width * _depth) + z * _width + x;

        public T GetAt(int x, int y, int z)
        {
            var pos = GetArrayPos(x, y, z);

            if (pos >= _mem.Length || pos < 0)
            {
                throw new IndexOutOfRangeException();
            }

            return _mem.Span[pos];
        }

        public void SetAt(int x, int y, int z, T value)
        {
            var pos = GetArrayPos(x, y, z);

            if (pos >= _mem.Length || pos < 0)
            {
                return;
            }

            _mem.Span[pos] = value;
        }

        public void FillLayer(int y, T value)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _depth; z++)
                {
                    SetAt(x, y, z, value);
                }
            }
        }
    }
}