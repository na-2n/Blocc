using System;
using Blocc.Engine.Common.Extensions;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Textures
{
    public class Texture2DArray : TextureArray
    {
        private readonly uint _width;
        private readonly uint _height;
        private readonly PixelFormat _pixelFormat;
        private readonly PixelType _pixelType;

        private int _layerIndex;

        public Texture2DArray(
            GL gl, InternalFormat format, uint width, uint height, uint layerCount, PixelFormat pixelFormat,
            PixelType pixelType = PixelType.UnsignedByte, TextureTarget type = TextureTarget.Texture2DArray,
            int level = 0, int border = 0) : base(gl, type)
        {
            _width = width;
            _height = height;
            _pixelFormat = pixelFormat;
            _pixelType = pixelType;

            Bind();

            unsafe
            {
                _gl.TexImage3D(
                    _type, level, (int) format, width, height, layerCount, border, pixelFormat, pixelType, null);
            }
        }

        public unsafe int AddTexture(void* data, int level, int x, int y)
        {
            _gl.TexSubImage3D(_type, level, x, y, _layerIndex, _width, _height, 1, _pixelFormat, _pixelType, data);

            return _layerIndex++;
        }

        public int AddTexture<T>(Span<T> data, int level, int x, int y)
            where T : unmanaged
        {
            unsafe
            {
                return AddTexture(data.AsVoidPtr(), level, x, y);
            }
        }

        public override unsafe int AddTexture(void* data)
            => AddTexture(data, 0, 0, 0);

        public override int AddTexture<T>(Span<T> data)
            => AddTexture(data, 0, 0, 0);
    }
}