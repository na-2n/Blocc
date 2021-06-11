using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blocc.Engine.IO
{
    public interface IFileSystem<T>
        where T : class
    {
        T Get(string path);

        Task<T> GetAsync(string path, CancellationToken cancelToken = default);

        Stream GetStream(string path);
    }

    public static class FileSystemExtensions
    {
        public static string GetText<T>(this IFileSystem<T> @this, string path)
            where T : class
        {
            using var stream = @this.GetStream(path);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static async Task<string> GetTextAsync<T>(this IFileSystem<T> @this, string path)
            where T : class
        {
            using var stream = @this.GetStream(path);
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}