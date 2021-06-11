using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenGL;
using Blocc.Engine.Common.Extensions;

namespace Blocc.Engine.OpenGL.Shaders
{
    public class ShaderProgram : IDisposable, IHasHandle
    {
        private readonly GL _gl;
        private readonly Stack<Shader> _shaders;

        private bool _isDisposed;

        public uint Handle { get; }

        public ShaderProgram(GL gl)
        {
            _gl = gl;

            _shaders = new Stack<Shader>();

            Handle = gl.CreateProgram();
        }

        public int GetUniformLocation(string name)
            => _gl.GetUniformLocation(Handle, name);

        public int this[string name] => GetUniformLocation(name);

        public void SetUniform(string name, ref Vector2 vec)
            => _gl.Uniform2(GetUniformLocation(name), vec);

        public void SetUniform(string name, ref Vector3 vec)
            => _gl.Uniform3(GetUniformLocation(name), vec);

        public void SetUniform(string name, ref Vector4 vec)
            => _gl.Uniform4(GetUniformLocation(name), vec);

        public void SetUniform(string name, Matrix4x4 mat)
        {
            var span = mat.AsSpan<float, Matrix4x4>();

            unsafe
            {
                _gl.UniformMatrix4(GetUniformLocation(name), 1, false, span.AsPtr());
            }
        }

        public void SetUniform(string name, int value)
            => _gl.Uniform1(GetUniformLocation(name), value);

        public void SetUniform(string name, uint value)
            => _gl.Uniform1(GetUniformLocation(name), value);

        public void SetUniform(string name, float value)
            => _gl.Uniform1(GetUniformLocation(name), value);

        public void Attach(Shader shader)
        {
            _gl.AttachShader(Handle, shader.Handle);

            _shaders.Push(shader);
        }

        public void Link()
        {
            _gl.LinkProgram(Handle);

            var log = _gl.GetProgramInfoLog(Handle);

            if (!string.IsNullOrWhiteSpace(log))
            {
                throw new Exception($"Failed to link shader program: {log}");
            }

            foreach (var shader in _shaders)
            {
                _gl.DetachShader(Handle, shader.Handle);
            }
        }

        public void Use() => _gl.UseProgram(Handle);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _gl.DeleteProgram(Handle);

                _isDisposed = true;
            }
        }

        ~ShaderProgram() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}