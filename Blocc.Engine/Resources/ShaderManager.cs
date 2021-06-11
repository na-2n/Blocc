using System.Collections.Generic;
using System.IO;
using Blocc.Engine.IO;
using Blocc.Engine.OpenGL.Shaders;
using Silk.NET.OpenGL;

namespace Blocc.Engine.Resources
{
    using ShaderStore = Dictionary<ShaderType, Dictionary<string, Shader>>;

    public class ShaderManager
    {
        private readonly IGame _game;
        private readonly IFileSystem<byte[]> _fs;
        private readonly ShaderStore _shaders;

        public ShaderManager(IGame game, IFileSystem<byte[]> fs)
        {
            _game = game;
            _fs = fs;

            _shaders = new ShaderStore
            {
                [ShaderType.VertexShader] = new Dictionary<string, Shader>(),
                [ShaderType.FragmentShader] = new Dictionary<string, Shader>(),
            };
        }

        public Shader Load(string path, ShaderType type)
        {
            var name = Path.GetFileName(path);

            if (_shaders[type].TryGetValue(name, out var shader))
            {
                return shader;
            }

            var source = _fs.GetText(path);

            shader = new Shader(_game.GL, source, type);

            return _shaders[type][name] = shader;
        }

        public ShaderProgram CreateProgram(string vertPath, string fragPath)
        {
            var vert = Load(vertPath, ShaderType.VertexShader);
            var frag = Load(fragPath, ShaderType.FragmentShader);

            var prog = new ShaderProgram(_game.GL);

            prog.Attach(vert);
            prog.Attach(frag);

            prog.Link();

            return prog;
        }
    }
}