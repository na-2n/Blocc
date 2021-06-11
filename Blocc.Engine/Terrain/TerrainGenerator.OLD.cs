using System;
using System.Collections.Concurrent;
using Blocc.Engine.Algorithm;
using Blocc.Engine.Algorithm.FastNoise;
using Blocc.Engine.Common;
using Blocc.Engine.World;

namespace Blocc.Engine.Terrain
{
    public class TerrainGenerator
    {
        private const float Scale = 0.0075f;
        public const int MaxHeight = 127;
        private const int Octaves = 16;

        private readonly INoiseAlgorithm _noise;
        private readonly ConcurrentDictionary<(int, int, int), bool> _cache; // BAD! REPLACE!
        private float _threshold;

        public TerrainGenerator(int seed, float threshold = Chunk.Height / MathF.PI)
        {
            _noise = new FastPerlin(seed);
            _cache = new ConcurrentDictionary<(int, int, int), bool>();
            _threshold = threshold;
        }

        public bool At(int x, int y, int z)
        {
            if (_cache.TryGetValue((x, y, z), out var ret))
            {
                return ret;
            }

            var depth = SumOctaves(Octaves, x, y, z, 0.25f, Scale, 0, MaxHeight, 1);

            depth *= BloccMath.ErfGradient(y, MaxHeight);

            return _cache[(x, y, z)] = (depth >= _threshold);
        }

        private float SumOctaves(int octaves, float x, float y, float z, float persistence, float scale, float min,
            float max, float smoothness = 10)
        {
            var maxAmplitude = 0f;
            var amplitude = 1f;
            var frequency = scale;
            var noise = 0f;

            for (var i = 0; i < octaves; i++)
            {
                var x1 = x * frequency / smoothness;
                var y1 = y * frequency / smoothness;
                var z1 = z * frequency / smoothness;

                noise += _noise[x1, y1, z1] * amplitude;

                maxAmplitude += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            noise /= maxAmplitude;

            return noise * (max - min) / 2 + (max - min) / 2;
        }
    }
}