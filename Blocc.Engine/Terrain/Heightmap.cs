using Blocc.Engine.Algorithm;
using Blocc.Engine.Algorithm.FastNoise;

namespace Blocc.Engine.Terrain
{
    public class Heightmap
    {
        public const float Scale = 0.0005f;

        private readonly int _maxHeight;
        private readonly INoiseAlgorithm _noise;

        public Heightmap(int seed, int maxHeight = 255)
        {
            _maxHeight = maxHeight;
            _noise = new FastPerlin(seed);
        }

        public float SumOctaves(int octaves, float x, float y, float persistence = 0.5f, float scale = Scale)
        {
            var accumAmp = 0f;
            var amp = 1f;
            var freq = scale;
            var noise = 0f;

            for (var i = 0; i < octaves; i++)
            {
                var xPos = (x * freq);
                var yPos = (y * freq);

                noise += _noise[xPos, yPos] * amp;

                accumAmp += amp;

                amp *= persistence;
                freq *= 2;
            }

            noise /= accumAmp;

            return noise;
        }

        public float At(float x, float y)
            => SumOctaves(16, x, y) * _maxHeight;
    }
}