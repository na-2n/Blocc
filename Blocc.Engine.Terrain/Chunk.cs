using Blocc.Engine.Common;

namespace Blocc.Engine.Terrain
{
    public class Chunk
    {
        /*
         * Blocks are stored as uint32, each uint32 takes up 4 bytes
         *
         * Chunks are 16x16xHEIGHT where HEIGHT will be set to 256, resulting in 4096 possible block locations per chunk
         *
         * If we store all blocks in an array we would get 4096 * 4 = 16384 bytes/chunk
         *
         */

        public const int Size = 16;
        public const int Height = 256;

        private readonly Array3D<uint> _blocks;

        public int X { get; }
        public int Z { get; }

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;

            _blocks = new Array3D<uint>(Size, Height, Size);
        }

        public uint GetBlock(int x, int y, int z)
            => _blocks[x, y, z];
    }
}