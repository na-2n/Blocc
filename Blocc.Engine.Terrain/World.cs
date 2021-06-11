using System;
using System.Numerics;

namespace Blocc.Engine.Terrain
{
    public class World
    {
        public const byte DefaultRenderDistance = 16;

        private readonly IChunkStore _chunks;

        private byte _renderDistance;

        public World()
        {
            _renderDistance = DefaultRenderDistance;

            _chunks = new InefficientChunkStore();
        }

        public WorldBlock GetBlockAt(int x, int y, int z)
        {
            var chunkX = x / Chunk.Size;
            var chunkZ = z / Chunk.Size;

            // TODO: get value from noise
            if (!_chunks.TryGetValue(x, z, out var chunk))
            {
                return null;
            }

            var blockX = x % Chunk.Size;
            var blockZ = x % Chunk.Size;

            var blockId = chunk.GetBlock(blockX, y, blockZ);

            return new WorldBlock(blockId, new Vector3(x, y, z));
        }
    }
}