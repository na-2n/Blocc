using System;
using Silk.NET.OpenGL;
using Blocc.Engine.Common.Extensions;

namespace Blocc.Engine.OpenGL.Textures
{
    public class Texture3D<T> : Texture3D
        where T : unmanaged
    {
        public unsafe Texture3D(
            GL gl, InternalFormat format, uint width, uint height, uint depth, PixelFormat pixelFormat,
            PixelType pixelType, Span<T> data, TextureTarget type = TextureTarget.Texture3D, int level = 0,
            int border = 0)
            : base(gl, format, width, height, depth, pixelFormat, pixelType, data.AsVoidPtr(), type, level, border)
        {
        }
    }

    public class Texture3D : Texture
    {
        public unsafe Texture3D(
            GL gl, InternalFormat format, uint width, uint height, uint depth, PixelFormat pixelFormat,
            PixelType pixelType, void* data, TextureTarget type = TextureTarget.Texture3D, int level = 0,
            int border = 0)
            : base(gl, type)
        {
            Bind();

            _gl.TexImage3D(_type, level, (int)format, width, height, depth, border, pixelFormat, pixelType, data);
        }
    }
}