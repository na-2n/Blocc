using System;
using System.Numerics;
using Blocc.Engine.World;

namespace Blocc.Engine.Terrain
{
    public class TerrainGenerator
    {
        private readonly NoiseGenerator _noiseGen;

        public int Seed => _noiseGen.Seed;

        public TerrainGenerator(int seed)
        {
            _noiseGen = new NoiseGenerator(seed);
        }

        private const int NoiseOctaves = 8;
        private const float DensityThreshold = 0.1f;

        private const float GroundScale = 1 / 128f; // 256
        private const float GroundFreq = 0.5f;
        private const float GroundAmp = 0.5f;
        private const float GroundMag = 32;
        private const float GroundOffset = 42;

        private const float OverhangScale = 1 / 64f; // 128
        private const float OverhangFreq = 0.25f;
        private const float OverhangAmp = 0.25f;
        private const float OverhangMag = 4;
        private const float OverhangOffset = 2;

        public void FillChunk(Chunk chunk)
        {
            var baseHeight = 0;

            for (var chunkX = 0; chunkX < Chunk.Size; chunkX++)
            {
                for (var chunkZ = 0; chunkZ < Chunk.Size; chunkZ++)
                {
                    var x = chunk.X * Chunk.Size + chunkX;
                    var z = chunk.Z * Chunk.Size + chunkZ;

                    var groundHeight =
                        (int) (_noiseGen.NoiseAt(NoiseOctaves, new Vector2(x, z), GroundFreq, GroundAmp, GroundScale) *
                            GroundMag + GroundOffset + baseHeight);

                    var maxHeight =
                        (int) (_noiseGen.NoiseAt(NoiseOctaves, new Vector2(x, z), OverhangFreq, OverhangAmp, OverhangScale) *
                            OverhangMag + OverhangOffset + groundHeight);

                    maxHeight = Math.Max(1, maxHeight);

                    for (var y = 0; y < maxHeight && y < Chunk.Height; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(chunkX, y, chunkZ, 0);

                            continue;
                        }

                        if (y > groundHeight)
                        {
                            var density = _noiseGen.NoiseAt(NoiseOctaves, new Vector3(x, y, z), OverhangFreq,
                                OverhangAmp, OverhangScale);

                            if (density > DensityThreshold)
                            {
                                chunk.SetBlock(chunkX, y, chunkZ, 2);
                            }
                        }
                        else
                        {
                            chunk.SetBlock(chunkX, y, chunkZ, 1);
                        }
                    }
                }
            }
        }
    }
}