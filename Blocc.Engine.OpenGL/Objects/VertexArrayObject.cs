using System;
using System.Linq;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Objects
{
    public class VertexArrayObject : IDisposable, IBindable, IHasHandle
    {
        private readonly GL _gl;
        private bool _isDisposed;

        public uint Handle { get; }
        public VertexAttr[] Attributes { get; }

        public unsafe VertexArrayObject(GL gl, VertexAttr.Info[] attrInfo)
        {
            _gl = gl;

            Handle = gl.GenVertexArray();

            Bind();

            var stride = attrInfo.Sum(info => info.Size * Marshal.SizeOf(info.Type));
            var dataSize = 0;

            Attributes = attrInfo.Select((info, i) =>
            {
                var pos = dataSize;

                dataSize += info.Size * Marshal.SizeOf(info.Type);

                return new VertexAttr(gl, info.Type, (uint)i, info.Size, (uint)stride, (void*)pos);
            }).ToArray();

            EnableAllAttrs();
        }

        public void EnableAttr(int index) => Attributes[index].Enable();

        public void EnableAllAttrs()
        {
            foreach (var attr in Attributes)
            {
                attr.Enable();
            }
        }

        public void DisableAttr(int index) => Attributes[index].Disable();

        public void DisableAllAttrs()
        {
            foreach (var attr in Attributes)
            {
                attr.Disable();
            }
        }

        public void Bind() => _gl.BindVertexArray(Handle);

        public void Unbind() => _gl.BindVertexArray(0);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _gl.DeleteVertexArray(Handle);

                _isDisposed = true;
            }
        }

        ~VertexArrayObject() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}