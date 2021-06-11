using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using Blocc.Engine.Renderer;
using Blocc.Engine.World;
using Blocc.Engine.Algorithm;
using System;
using Blocc.Engine.Terrain;

namespace Blocc.Engine.Threading
{
    public class ChunkGenerator
    {
        private readonly IGame _game;
        private readonly ConcurrentBag<Chunk> _chunks;
        private readonly TerrainGenerator _terrainGen;

        private bool _active;
        private Thread _thread;

        public int RenderDistance { get; set; }

        public ChunkGenerator(IGame game)
        {
            _game = game;
            _chunks = new ConcurrentBag<Chunk>();
            _terrainGen = new TerrainGenerator((int)DateTimeOffset.Now.ToUnixTimeSeconds());

            RenderDistance = 16;
        }

        public void Start(Camera cam)
        {
            _active = true;

            _thread = new Thread(() => Run(cam));

            _thread.Start();
        }

        public void Run(Camera cam)
        {
            while (_active)
            {
                var x = (int)cam.Position.X;
                var z = (int)cam.Position.Z;

                GenAt(cam);

                Thread.Sleep(1);
            }
        }

        public void Stop()
        {
            _active = false;

            _thread.Join();
        }

        public int GetBlock(int x, int y, int z)
        {
            var chunkX = x / Chunk.Size;
            var chunkZ = z / Chunk.Size;

            var chunk = _chunks.FirstOrDefault(c => c.X == chunkX && c.Z == chunkZ);

            if (chunk != null)
            {
                return chunk.GetBlock(x % Chunk.Size, y, z % Chunk.Size);
            }

            return -1;
        }

        public void GenAt(Camera cam)
        {
            var ix = 0;
            var iz = 0;
            var dx = 0;
            var dz = -1;

            var dist = RenderDistance * 2;
            var maxI = dist * dist;

            var lastX = cam.Position.X;
            var lastZ = cam.Position.Z;

            for (var i = 0; i < maxI; i++)
            {
                if (!_active)
                {
                    return;
                }

                if (lastX != cam.Position.X || lastZ != cam.Position.Z)
                {
                    GenAt(cam);

                    return;
                }

                if ((-dist / 2 <= ix) && (ix <= dist / 2) && (-dist / 2 <= iz) && (iz <= dist / 2))
                {
                    var posX = ix + ((int)cam.Position.X / Chunk.Size);
                    var posZ = iz + ((int)cam.Position.Z / Chunk.Size);

                    if (!_chunks.Any(c => c.X == posX && c.Z == posZ))
                    {
                        Chunk chunk;

                        _chunks.Add(chunk = new Chunk(_game, posX, posZ, _terrainGen));

                        chunk.GenerateMesh();
                    }
                }

                if ((ix == iz) || ((ix < 0) && (ix == -iz)) || ((ix > 0) && (ix == 1 - iz)))
                {
                    var tmp = dx;

                    dx = -dz;
                    dz = tmp;
                }

                ix += dx;
                iz += dz;
            }
        }

        public void Render(int x, int z)
        {
            var posX = x / Chunk.Size;
            var posZ = z / Chunk.Size;
            var minX = posX - RenderDistance;
            var maxX = posX + RenderDistance;
            var minZ = posZ - RenderDistance;
            var maxZ = posZ + RenderDistance;

            foreach (var chunk in _chunks)
            {
                if (chunk.X < minX || chunk.X > maxX ||
                    chunk.Z < minZ || chunk.Z > maxZ)
                {
                    continue;
                }

                if (chunk.HasMesh)
                {
                    if (!chunk.BlockMesh.IsReady)
                    {
                        chunk.BlockMesh.Setup();
                    }

                    chunk.Draw();
                }
            }
        }
    }
}