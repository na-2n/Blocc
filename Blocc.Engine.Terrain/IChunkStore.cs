namespace Blocc.Engine.Terrain
{
    public interface IChunkStore
    {
        Chunk this[int x, int z] => GetValue(x, z);

        void Add(Chunk chunk);

        Chunk GetValue(int x, int z);

        bool TryGetValue(int x, int z, out Chunk chunkOut);
    }
}