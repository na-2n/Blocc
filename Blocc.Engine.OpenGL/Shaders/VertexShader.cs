using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Shaders
{
    public sealed class VertexShader : Shader
    {
        public VertexShader(GL gl, string source) : base(gl, source, ShaderType.VertexShader)
        { }
    }
}