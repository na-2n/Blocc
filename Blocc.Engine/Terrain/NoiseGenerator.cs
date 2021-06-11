using System.Numerics;
using Blocc.Engine.Algorithm;
using Blocc.Engine.Algorithm.FastNoise;

namespace Blocc.Engine.Terrain
{
    public class NoiseGenerator
    {
        private readonly INoiseAlgorithm _noise;

        public int Seed => _noise.Seed;

        public NoiseGenerator(int seed)
        {
            _noise = new FastSimplex(seed);
        }

        public float NoiseAt(int octaves, Vector2 pos, float freqMultiplier, float ampMultiplier, float scale = 1,
            bool normalized = false)
        {
            float res = 0;
            float amp = 1;
            float accumAmp = 0;
            float freq = 1;

            pos *= scale;

            for (var i = 0; i < octaves; i++)
            {
                res += _noise.At(pos.X * freq, pos.Y * freq) * amp;

                accumAmp += amp;

                freq *= freqMultiplier;
                amp *= ampMultiplier;
            }

            if (normalized)
            {
                res /= accumAmp;
            }

            return res;
        }

        public float NoiseAt(int octaves, Vector3 pos, float freqMultiplier, float ampMultiplier, float scale = 1,
            bool normalized = false)
        {
            float res = 0;
            float amp = 1;
            float accumAmp = 0;
            float freq = 1;

            pos *= scale;

            for (var i = 0; i < octaves; i++)
            {
                res += _noise.At(pos.X * freq, pos.Y * freq, pos.Z * freq) * amp;

                accumAmp += amp;

                freq *= freqMultiplier;
                amp *= ampMultiplier;
            }

            if (normalized)
            {
                res /= accumAmp;
            }

            return res;
        }
    }
}