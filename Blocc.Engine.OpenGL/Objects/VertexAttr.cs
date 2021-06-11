using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Objects
{
    public sealed class VertexAttr<TData> : VertexAttr
    {
        internal unsafe VertexAttr(GL gl, uint index, int size, uint stride, void* dataPtr, bool normalized = false)
            : base(gl, FromClrType<TData>(), index, size, stride, dataPtr, normalized)
        { }
    }

    public class VertexAttr
    {
        public sealed class Info<TData> : Info
        {
            public Info() : base(typeof(TData))
            { }
        }

        public class Info
        {
            public int Size { get; set; }

            public bool Normalized { get; set; } = false;

            internal Type Type;

            public Info()
            {
                Type = typeof(float);
            }

            public Info(Type type)
            {
                Type = type;
            }
        }

        private readonly GL _gl;

        public uint Index { get; }

        internal unsafe VertexAttr(GL gl, Type dataType, uint index, int size, uint stride, void* dataPtr,
            bool normalized = false)
                : this(gl, FromClrType(dataType), index, size, stride, dataPtr, normalized)
        { }

        internal unsafe VertexAttr(GL gl, VertexAttribPointerType dataType, uint index, int size, uint stride,
            void* dataPtr, bool normalized = false)
        {
            _gl = gl;
            Index = index;

            gl.VertexAttribPointer(index, size, dataType, normalized, stride, dataPtr);
        }

        public void Enable() => _gl.EnableVertexAttribArray(Index);

        public void Disable() => _gl.DisableVertexAttribArray(Index);

        internal static readonly Dictionary<Type, VertexAttribPointerType> TypeConv
            = new Dictionary<Type, VertexAttribPointerType>
            {
                [typeof(sbyte)] = VertexAttribPointerType.Byte,
                [typeof(byte)] = VertexAttribPointerType.UnsignedByte,
                [typeof(short)] = VertexAttribPointerType.Short,
                [typeof(ushort)] = VertexAttribPointerType.UnsignedShort,
                [typeof(int)] = VertexAttribPointerType.Int,
                [typeof(uint)] = VertexAttribPointerType.UnsignedInt,
                [typeof(float)] = VertexAttribPointerType.Float,
                [typeof(double)] = VertexAttribPointerType.Double,
            };

        internal static VertexAttribPointerType FromClrType(Type type)
            => TypeConv.GetValueOrDefault(type, VertexAttribPointerType.Byte);

        internal static VertexAttribPointerType FromClrType<T>()
            => FromClrType(typeof(T));
    }
}