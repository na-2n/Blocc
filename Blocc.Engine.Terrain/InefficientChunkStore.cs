using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Blocc.Engine.Terrain
{
    public class InefficientChunkStore : IChunkStore
    {
        private readonly ConcurrentDictionary<(int, int), Chunk> _chunks;

        public InefficientChunkStore()
        {
            _chunks = new ConcurrentDictionary<(int, int), Chunk>();
        }

        public void Add(Chunk chunk)
        {
            _chunks[(chunk.X, chunk.Z)] = chunk;
        }

        public Chunk GetValue(int x, int z)
        {
            if (TryGetValue(x, z, out var ret))
            {
                return ret;
            }

            throw new KeyNotFoundException($"Could not find chunk at X = {x}, Z = {z}");
        }

        public bool TryGetValue(int x, int z, out Chunk chunkOut)
            => _chunks.TryGetValue((x, z), out chunkOut);
    }
}