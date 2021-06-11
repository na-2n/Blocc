using System;
using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Shaders
{
    public class Shader : IDisposable, IHasHandle
    {
        private readonly GL _gl;
        private bool _isDisposed;

        public uint Handle { get; }

        public Shader(GL gl, string source, ShaderType type)
        {
            _gl = gl;

            Handle = gl.CreateShader(type);

            gl.ShaderSource(Handle, source);
            gl.CompileShader(Handle);

            var log = gl.GetShaderInfoLog(Handle);

            if (!string.IsNullOrWhiteSpace(log))
            {
                throw new Exception($"Failed to compile shader of type `{type}': {log}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _gl.DeleteShader(Handle);

                _isDisposed = true;
            }
        }

        ~Shader() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}