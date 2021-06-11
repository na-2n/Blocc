using System.Numerics;

namespace Blocc.Engine.Terrain
{
    public class WorldBlock
    {
        public uint Id { get; }
        public Vector3 Position { get; }
        public Direction Direction { get; }

        public WorldBlock(uint id, Vector3 pos, Direction dir = Direction.North)
        {
            Id = id;
            Position = pos;
            Direction = dir;
        }
    }
}