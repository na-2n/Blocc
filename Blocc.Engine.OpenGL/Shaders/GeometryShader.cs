using Silk.NET.OpenGL;

namespace Blocc.Engine.OpenGL.Shaders
{
    public sealed class GeometryShader : Shader
    {
        public GeometryShader(GL gl, string source) : base(gl, source, ShaderType.GeometryShader)
        { }
    }
}