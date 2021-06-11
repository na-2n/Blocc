using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Shaders
{
    public sealed class FragmentShader : Shader
    {
        public FragmentShader(GL gl, string source) : base(gl, source, ShaderType.FragmentShader)
        { }
    }
}