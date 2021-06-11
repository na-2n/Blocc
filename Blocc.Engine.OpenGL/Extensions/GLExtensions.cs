using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Extensions
{
    public static class GLExtensions
    {
        public static void Clear(this GL @this, ClearBufferMask mask)
            => @this.Clear((uint)mask);
    }
}