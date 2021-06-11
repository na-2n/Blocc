using System;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Textures
{
    public abstract class Texture : IDisposable, IBindable, IHasHandle
    {
        protected readonly GL _gl;
        protected readonly TextureTarget _type;

        private bool _isDisposed;

        public uint Handle { get; }

        public float this[TextureParameterName name]
        {
            set => SetParameter(name, value);
        }

        protected Texture(GL gl, TextureTarget type)
        {
            _gl = gl;
            _type = type;

            Handle = gl.GenTexture();
        }

        public void GenMipMap()
            => _gl.GenerateMipmap(_type);

        public void SetParameter(TextureParameterName name, int value)
            => _gl.TexParameter(_type, name, value);

        public void SetParameter(TextureParameterName name, float value)
            => _gl.TexParameter(_type, name, value);

        public void SetParameter(TextureParameterName name, ref int value)
            => _gl.TexParameter(_type, name, ref value);

        public void SetParameter(TextureParameterName name, ref float value)
            => _gl.TexParameter(_type, name, ref value);

        public unsafe void SetParameterI(TextureParameterName name, int* value)
            => _gl.TexParameterI(_type, name, value);

        public unsafe void SetParameterI(TextureParameterName name, uint* value)
            => _gl.TexParameterI(_type, name, value);

        public void SetParameterI(TextureParameterName name, ref int value)
            => _gl.TexParameterI(_type, name, ref value);

        public void SetParameterI(TextureParameterName name, ref uint value)
            => _gl.TexParameterI(_type, name, ref value);

        public void Bind(TextureUnit unit)
        {
            _gl.ActiveTexture(unit);
            
            _gl.BindTexture(_type, Handle);
        }

        public void Bind()
            => Bind(TextureUnit.Texture0);

        public void Unbind()
            => _gl.BindTexture(_type, 0);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _gl.DeleteTexture(Handle);

                _isDisposed = true;
            }
        }

        ~Texture() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}