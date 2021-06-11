using System.Numerics;
using Blocc.Engine.Renderer;

namespace Blocc.Engine.World
{
    public class Player
    {
        private readonly Camera _cam;

        public Vector3 Position { get; }

        public Player(Camera cam)
        {
            _cam = cam;
        }

        public void CollideWith(Block block)
        {

        }
    }
}