using System.Numerics;
using Blocc.Engine.Common;
using Blocc.Engine.Renderer;
using Blocc.Engine.Terrain;

namespace Blocc.Engine.World
{
    public class Chunk
    {
        public const int Size = 16;
        public const int Height = 256;

        private readonly IGame _game;
        private readonly Array3D<int> _blocks;
        private readonly TerrainGenerator _terrainGen;

        public Mesh BlockMesh { get; }

        public int X { get; }
        public int Z { get; }

        public bool IsChanged { get; private set; }
        public bool HasMesh { get; private set; }

        public Chunk(IGame game, int x, int z, TerrainGenerator terrainGen)
        {
            _game = game;
            _blocks = new Array3D<int>(Size, Height, Size);
            _terrainGen = terrainGen;

            BlockMesh = new Mesh(_game.GL);
            X = x;
            Z = z;
            IsChanged = true;
            HasMesh = false;

            _blocks.Span.Fill(-1);

            _terrainGen.FillChunk(this);

            //_blocks.FillLayer(0, 2);
        }

        /*private void Fill()
        {
            for (var x = 0; x < Size; x++)
            {
                for (var z = 0; z < Size; z++)
                {
                    var xPos = x + (X * Size);
                    var zPos = z + (Z * Size);

                    var factor = _terrainGen.SumOctaves(16, xPos, zPos);
                    var maxY = (int)(Height * factor);

                    var blockType = factor switch
                    {
                        var a when a < 0 => -1,
                        var a when a < 0.0625 => _game.BlockManager.GetBlockId("sand"),
                        var a when a < 0.125 => _game.BlockManager.GetBlockId("grass"),
                        //var a when a < 0.375 => _game.BlockManager.GetBlockId("dirt"),
                        _ => _game.BlockManager.GetBlockId("stone")
                    };

                    _blocks[x, maxY, z] = blockType;

                    for (var y = 0; y < maxY; y++)
                    {
                        _blocks[x, y, z] = _game.BlockManager.GetBlockId("stone");
                    }
                }
            }
        }*/

        /*private void Fill()
        {
            for (var z = 0; z < ChunkSize; z++)
            {
                for (var x = 0; x < ChunkSize; x++)
                {
                    for (var y = 0; y < ChunkHeight; y++)
                    {
                        var xPos = x + (ChunkX * ChunkSize);
                        var zPos = z + (ChunkZ * ChunkSize);

                        if (_terrainGen.At(xPos, y, zPos))
                        {
                            if (_terrainGen.At(xPos, y + 1, zPos))
                            {
                                _blocks[x, y, z] = y < 64 ? 2 : 0;
                            }
                            else
                            {
                                _blocks[x, y, z] = 1;
                            }
                        }
                    }
                }
            }
        }*/

        public bool Destroy(int x, int y, int z)
        {
            if (GetBlock(x, y, z) == -1)
            {
                return false;
            }

            SetBlock(x, y, z, -1);

            return true;
        }

        public int GetBlock(int x, int y, int z)
        {
            if ((x < 0 || y < 0 || z < 0) ||
                (x >= Size || y >= Height || z >= Size))
            {
                /*var xPos = x + (X * Size);
                var zPos = z + (Z * Size);
                var factor = _terrainGen.SumOctaves(16, xPos, zPos);
                var maxY = (int)(Height * factor);

                if (y <= maxY)
                {
                    return 0;
                }*/

                return -1;
            }

            return _blocks[x, y, z];
        }

        public void SetBlock(int x, int y, int z, int blockId)
        {
            _blocks[x, y, z] = blockId;

            IsChanged = true;
        }

        private bool ShouldRenderSide(int id, int otherId)
        {
            if (otherId == -1)
            {
                return true;
            }

            return false;
        }

        public void Update()
        {
            if (IsChanged)
            {
                BlockMesh.Update();

                IsChanged = false;
            }
        }

        public void Draw(bool canUpdate = true)
        {
            if (BlockMesh.IsReady)
            {
                if (canUpdate)
                {
                    Update();
                }

                BlockMesh.Draw();
            }
        }

        public void GenerateMesh()
        {
            if (!IsChanged)
            {
                return;
            }

            BlockMesh.Reset();

            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    for (var z = 0; z < Size; z++)
                    {
                        var blockId = GetBlock(x, y, z);

                        if (blockId == -1)
                        {
                            continue;
                        }

                        var pos = new Vector3(x + (X * Size), y, z + (Z * Size));

                        var block = _game.BlockManager.GetBlock(blockId);

                        var overlayColor = new Vector3(0, 1, 0);

                        var sideOverlay = new Vector4(overlayColor, 1);
                        var topOverlay = new Vector4(overlayColor, 3);
                        var noOverlay = new Vector4(Vector3.Zero, -1);

                        // Left side
                        if (ShouldRenderSide(blockId, GetBlock(x - 1, y, z)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureLeft);

                            BlockMesh.AddFace(Block.FaceLeft, pos, texId, noOverlay);
                        }

                        // Right side
                        if (ShouldRenderSide(blockId, GetBlock(x + 1, y, z)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureRight);

                            BlockMesh.AddFace(Block.FaceRight, pos, texId, noOverlay);
                        }

                        // Front side
                        if (ShouldRenderSide(blockId, GetBlock(x, y, z + 1)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureFront);

                            BlockMesh.AddFace(Block.FaceFront, pos, texId, noOverlay);
                        }

                        // Back side
                        if (ShouldRenderSide(blockId, GetBlock(x, y, z - 1)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureBack);

                            BlockMesh.AddFace(Block.FaceBack, pos, texId, noOverlay);
                        }

                        // Bottom side
                        if (ShouldRenderSide(blockId, GetBlock(x, y - 1, z)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureBottom);

                            BlockMesh.AddFace(Block.FaceBottom, pos, texId, noOverlay);
                        }

                        // Top side
                        if (ShouldRenderSide(blockId, GetBlock(x, y + 1, z)))
                        {
                            var texId = _game.TextureManager.GetTextureId(block.TextureTop);

                            BlockMesh.AddFace(Block.FaceTop, pos, texId, noOverlay);
                        }
                    }
                }
            }

            HasMesh = true;
        }
    }
}