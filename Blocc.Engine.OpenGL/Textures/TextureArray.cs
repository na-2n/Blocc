using System;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Textures
{
    public abstract class TextureArray : Texture
    {
        protected TextureArray(GL gl, TextureTarget type) : base(gl, type)
        {
        }

        public abstract unsafe int AddTexture(void* data);

        public abstract int AddTexture<T>(Span<T> data)
            where T : unmanaged;
    }
}