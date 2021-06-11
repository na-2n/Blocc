using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Blocc.Engine.IO;
using Blocc.Engine.OpenGL.Textures;
using Silk.NET.OpenGL;
using System.Collections.Concurrent;
using SixLabors.ImageSharp.Advanced;

namespace Blocc.Engine.Resources
{
    public class TextureManager
    {
        private readonly IGame _game;
        private readonly IFileSystem<byte[]> _fs;
        private readonly ConcurrentDictionary<string, int> _storedTextures;

        public TextureArray TextureArray { get; }

        public TextureManager(IGame game, IFileSystem<byte[]> fs, uint texSize = 16)
        {
            _game = game;
            _fs = fs;

            TextureArray = new Texture2DArray(game.GL, InternalFormat.Rgba, texSize, texSize, 256, PixelFormat.Rgba);
            _storedTextures = new ConcurrentDictionary<string, int>();
        }

        public Image<T> LoadImage<T>(string path)
            where T : struct, IPixel<T>
        {
            var data = _fs.Get(path);
            var img = Image.Load<T>(data);

            img.Mutate(x => x.Flip(FlipMode.Vertical));

            return img;
        }

        public Image<T> LoadImage<T>(string path, out InternalFormat format)
            where T : struct, IPixel<T>
        {
            var img = LoadImage<T>(path);

            format = (img.PixelType.BitsPerPixel / 8) switch
            {
                1 => InternalFormat.Red,
                2 => InternalFormat.RG,
                3 => InternalFormat.Rgb,
                4 => InternalFormat.Rgba,
                var x => throw new InvalidOperationException($"Invalid pixel size : {x}"),
            };

            return img;
        }

        public Image<Rgba32> LoadImage(string path)
            => LoadImage<Rgba32>(path);

        public Image<Rgba32> LoadImage(string path, out InternalFormat format)
            => LoadImage<Rgba32>(path, out format);

        public int LoadTexture(string path)
        {
            var img = LoadImage(path);

            return _storedTextures[path] = TextureArray.AddTexture(img.GetPixelSpan());
        }
            
        public int GetTextureId(string path)
        {
            if (_storedTextures.TryGetValue(path, out var id))
            {
                return id;
            }

            return -1;
        }
    }
}