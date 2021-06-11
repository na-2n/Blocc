namespace Blocc.Engine.World
{
    public interface IBlock
    {
        string Name { get; }

        string TextureFront { get; }

        string TextureBack => TextureFront;

        string TextureLeft => TextureFront;

        string TextureRight => TextureFront;

        string TextureTop => TextureFront;

        string TextureBottom => TextureFront;
    }
}