using System;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Objects
{
    public sealed class VertexBufferObject<T> : BufferObject<T>
        where T : unmanaged
    {
        public VertexBufferObject(GL gl, Span<T> data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
            : base(gl, data, BufferTargetARB.ArrayBuffer, usage)
        { }
    }

    public class VertexBufferObject : BufferObject
    {
        public unsafe VertexBufferObject(
            GL gl, uint dataSize, void* data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
            : base(gl, dataSize, data, BufferTargetARB.ArrayBuffer, usage)
        { }
    }
}