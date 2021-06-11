using System;
using Silk.NET.OpenGL;
using Blocc.Engine.Common.Extensions;

namespace Blocc.Engine.OpenGL.Textures
{
    public class Texture2D<T> : Texture2D
        where T : unmanaged
    {
        public unsafe Texture2D(
            GL gl, InternalFormat format, uint width, uint height, PixelFormat pixelFormat, PixelType pixelType,
            Span<T> data, TextureTarget type = TextureTarget.Texture2D, int level = 0, int border = 0)
            : base(gl, format, width, height, pixelFormat, pixelType, data.AsVoidPtr(), type, level, border)
        {
        }
    }

    public class Texture2D : Texture
    {
        public unsafe Texture2D(
            GL gl, InternalFormat format, uint width, uint height, PixelFormat pixelFormat, PixelType pixelType,
            void* data, TextureTarget type = TextureTarget.Texture2D, int level = 0, int border = 0)
            : base(gl, type)
        {
            Bind();

            _gl.TexImage2D(_type, level, (int)format, width, height, border, pixelFormat, pixelType, data);
        }
    }
}