using System.Numerics;
using Blocc.Engine.Renderer;

namespace Blocc.Engine.World
{
    public class Block : IBlock
    {
        #region Meshes

        public static Mesh.Face FaceFront => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
            }
        };

        public static Mesh.Face FaceBack => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),
            }
        };

        public static Mesh.Face FaceRight => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),
            }
        };

        public static Mesh.Face FaceLeft => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
            }
        };

        public static Mesh.Face FaceTop => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
            }
        };

        public static Mesh.Face FaceBottom => new Mesh.Face
        {
            Vertices = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
            }
        };

        #endregion
        
        public string Name { get; set; }

        #region Textures

        public string TextureFront { get; set; }

        public string TextureBack { get; set; }

        public string TextureLeft { get; set; }

        public string TextureRight { get; set; }

        public string TextureTop { get; set; }

        public string TextureBottom { get; set; }

        #endregion
    }
}