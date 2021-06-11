using System;
using System.Collections.Generic;
using Blocc.Engine.World;

namespace Blocc.Engine.Resources
{
    public class BlockManager
    {
        private readonly List<IBlock> _blockStore;
        private readonly TextureManager _texManager;

        public BlockManager(TextureManager texManager)
        {
            _blockStore = new List<IBlock>();
            _texManager = texManager;
        }

        public void PreloadTextures()
        {
            _texManager.TextureArray.Bind();

            foreach (var block in _blockStore)
            {
                _texManager.LoadTexture(block.TextureFront);
                _texManager.LoadTexture(block.TextureBack);
                _texManager.LoadTexture(block.TextureLeft);
                _texManager.LoadTexture(block.TextureRight);
                _texManager.LoadTexture(block.TextureTop);
                _texManager.LoadTexture(block.TextureBottom);
            }
        }

        public void StoreBlock(IBlock block)
            => _blockStore.Add(block);

        public int GetBlockId(string block)
            => _blockStore.FindIndex(x => x.Name == block);

        public IBlock GetBlock(string block)
            => _blockStore.Find(x => x.Name == block);

        public IBlock GetBlock(int blockId)
            => _blockStore[blockId];
    }
}