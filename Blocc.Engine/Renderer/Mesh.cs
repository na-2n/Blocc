using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Blocc.Engine.OpenGL.Objects;
using Silk.NET.OpenGL;

namespace Blocc.Engine.Renderer
{
    public class Mesh
    {
        public const int FaceVertices = 4;

        public ref struct Face
        {
            public ReadOnlySpan<Vector3> Vertices { get; set; }
        }

        private readonly GL _gl;

        private readonly List<Vertex> _vertices;
        private readonly List<uint> _indices;

        private uint _indexCount;

        public VertexArrayObject VAO { get; private set; }
        public BufferObject VBO { get; private set; }
        public BufferObject EBO { get; private set; }

        public bool IsReady { get; private set; }

        public Mesh(GL gl)
        {
            _gl = gl;
            
            _vertices = new List<Vertex>();
            _indices = new List<uint>();
        }

        public void AddFace(Face face, Vector3 pos, float textureId, Vector4 overlay)
        {
            if (face.Vertices.Length != FaceVertices)
            {
                throw new ArgumentOutOfRangeException(nameof(face), $"Vertex count must be equal to {FaceVertices}");
            }

            for (var i = 0; i < FaceVertices; i++)
            {
                var vertexPos = face.Vertices[i];
                
                _vertices.Add(new Vertex
                {
                    Position = vertexPos + pos,
                    Index = i,
                    Texture = textureId,
                    OverlayColor = new Vector3(overlay.X, overlay.Y, overlay.Z),
                    OverlayTexture = overlay.W,
                });
            }

            _indices.Add(_indexCount);
            _indices.Add(_indexCount + 1);
            _indices.Add(_indexCount + 2);
            _indices.Add(_indexCount + 2);
            _indices.Add(_indexCount + 3);
            _indices.Add(_indexCount);

            _indexCount += 4;
        }

        private byte[] GetVertBytes()
            => _vertices.SelectMany(v =>
            {
                var size = Marshal.SizeOf<Vertex>();
                var buf = new byte[size];

                MemoryMarshal.Write(buf, ref v);

                return buf;
            }).ToArray();

        public void Setup()
        {
            var vertArr = GetVertBytes();

            VBO = new VertexBufferObject<byte>(_gl, vertArr);
            VAO = new VertexArrayObject(_gl, new VertexAttr.Info[]
            {
                new VertexAttr.Info<float> { Size = 3 },
                new VertexAttr.Info<float> { Size = 1 },
                new VertexAttr.Info<float> { Size = 1 },
                new VertexAttr.Info<float> { Size = 3 },
                new VertexAttr.Info<float> { Size = 1 },
            });
            EBO = new ElementBufferObject<uint>(_gl, _indices.ToArray());

            IsReady = true;
        }

        public void Bind()
        {
            VBO.Bind();
            VAO.Bind();
            EBO.Bind();
        }

        public void Update()
        {
            VBO.UpdateData<byte>(GetVertBytes());
            EBO.UpdateData<uint>(_indices.ToArray());
        }

        public void Reset()
        {
            _vertices.Clear();
            _indices.Clear();

            _indexCount = 0;
        }

        public void Draw()
        {
            Bind();

            unsafe
            {
                _gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Count, DrawElementsType.UnsignedInt, null);
            }
        }
    }
}