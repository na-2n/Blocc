using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Blocc.Engine.IO
{
    public sealed class AssemblyFileSystem : IFileSystem<byte[]>
    {
        private readonly Assembly _assembly;
        private readonly string _namespace;

        public AssemblyFileSystem(Assembly assembly)
        {
            _assembly = assembly;
            _namespace = assembly.GetName().Name;
        }

        public Stream GetStream(string path)
        {
            path = GetNamespacePath(path);

            var stream = _assembly.GetManifestResourceStream($"{_namespace}.{path}");

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource not found in assembly {_namespace}", path);
            }

            return stream;
        }

        public byte[] Get(string path)
        {
            using var stream = GetStream(path);

            var buf = new byte[(int)stream.Length];

            stream.Read(buf);

            return buf;
        }

        public async Task<byte[]> GetAsync(string path, CancellationToken cancelToken)
        {
            using var stream = GetStream(path);

            var buf = new byte[(int)stream.Length];

            await stream.ReadAsync(buf, cancelToken);

            return buf;
        }

        private static string GetNamespacePath(string path)
        {
            path = path.Replace('/', '.').Replace('-', '_');

            if (path[^1] == '.')
            {
                path = path[..^1];
            }

            return path;
        }
    }
}