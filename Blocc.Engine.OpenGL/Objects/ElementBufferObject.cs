using System;
using Silk.NET.OpenGL;
using Blocc.Engine.Common.Extensions;

namespace Blocc.Engine.OpenGL.Objects
{
    public sealed class ElementBufferObject<T> : BufferObject<T>
        where T : unmanaged
    {
        public ElementBufferObject(GL gl, Span<T> data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
            : base(gl, data, BufferTargetARB.ElementArrayBuffer, usage)
        { }
    }

    public class ElementBufferObject : BufferObject
    {
        public unsafe ElementBufferObject(
            GL gl, uint dataSize, void* data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
            : base(gl, dataSize, data, BufferTargetARB.ElementArrayBuffer, usage)
        { }
    }
}