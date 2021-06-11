using System;
using System.Drawing;
using Blocc.Engine.Configuration;
using Blocc.Engine.Resources;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;

namespace Blocc.Engine
{
    public interface IGame
    {
        event Action<Key> KeyDown;
        event Action<PointF> CursorMove;

        GL GL { get; }
        BlockManager BlockManager { get; }
        TextureManager TextureManager { get; }
        IOptions Options { get; }
        Size ScreenSize { get; }
        double DeltaTime { get; }

        float DeltaTimeF => (float)DeltaTime;
    }
}