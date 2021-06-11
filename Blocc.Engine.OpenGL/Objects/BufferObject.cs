using System;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Blocc.Engine.OpenGL.Objects
{
    public class BufferObject<T> : BufferObject
        where T : unmanaged
    {
        public BufferObject(
            GL gl, Span<T> data, BufferTargetARB type, BufferUsageARB usage = BufferUsageARB.StaticDraw)
            : base(gl, type, usage)
        {
            UpdateData(data);
        }
    }

    public class BufferObject : IDisposable, IBindable, IHasHandle
    {
        private readonly BufferTargetARB _type;
        private readonly BufferUsageARB _usage;
        private bool _isDisposed;
        protected readonly GL _gl;

        public uint Handle { get; }

        public BufferObject(GL gl, BufferTargetARB type, BufferUsageARB usage)
        {
            _gl = gl;
            _type = type;
            _usage = usage;

            Handle = _gl.GenBuffer();
        }

        public unsafe BufferObject(GL gl, uint dataSize, void* data, BufferTargetARB type, BufferUsageARB usage)
            : this(gl, type, usage)
        {
            UpdateData(dataSize, data);
        }

        public unsafe void UpdateData(uint dataSize, void* data)
        {
            Bind();

            _gl.BufferData(_type, dataSize, data, _usage);
        }

        public void UpdateData<T>(Span<T> data)
            where T : unmanaged
        {
            Bind();

            _gl.BufferData(_type, (uint)(Marshal.SizeOf<T>() * data.Length), data, _usage);
        }

        public void Bind() => _gl.BindBuffer(_type, Handle);

        public void Unbind() => _gl.BindBuffer(_type, 0);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _gl.DeleteBuffer(Handle);

                _isDisposed = true;
            }
        }

        ~BufferObject() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}