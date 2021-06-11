using System;
using System.Drawing;
using System.Numerics;
using Blocc.Desktop.Configuration;
using Blocc.Engine;
using Blocc.Engine.Configuration;
using Blocc.Engine.IO;
using Blocc.Engine.OpenGL.Extensions;
using Blocc.Engine.OpenGL.Shaders;
using Blocc.Engine.Renderer;
using Blocc.Engine.Resources;
using Blocc.Engine.Threading;
using Blocc.Engine.World;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

namespace Blocc.Desktop
{
    public class DesktopGame : IGame
    {
        public const int ChunkSize = 16;
        public static APIVersion GLVersion => new APIVersion(4, 2);

        public event Action<Key> KeyDown;
        public event Action<PointF> CursorMove;

        private readonly IWindow _win;
        private readonly IFileSystem<byte[]> _fs;

        private IInputContext _input;
        private IKeyboard _kb;

        private Glfw _glfw;
        private ShaderManager _shaderManager;
        private Camera _camera;
        private ShaderProgram _shader;
        private ChunkGenerator _chunkGen;

        private bool _wireframe;

        public GL GL { get; private set; }
        public TextureManager TextureManager { get; private set; }
        public BlockManager BlockManager { get; private set; }
        public double DeltaTime { get; private set; }

        public IOptions Options { get; }

        public Size ScreenSize => _win.Size;

        public bool Wireframe
        {
            set
            {
                if (_wireframe = value)
                {
                    GL.PolygonMode((GLEnum)ColorMaterialFace.FrontAndBack, PolygonMode.Line);
                }
                else
                {
                    GL.PolygonMode((GLEnum)ColorMaterialFace.FrontAndBack, PolygonMode.Fill);
                }
            }

            get => _wireframe;
        }

        public DesktopGame()
        {
            _glfw = Glfw.GetApi();

            var options = WindowOptions.Default;

            options.Title = "Blocc";
            options.Size = new Size(800, 600);
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible,
                GLVersion);

            _win = Window.Create(options);
            _fs = new AssemblyFileSystem(Resources.Assembly);

            Options = new DesktopOptions();

            _win.Load += OnLoad;
            _win.Render += OnRender;
            _win.Resize += size => GL.Viewport(size);
            _win.Closing += OnClose;
        }

        public void Run() => _win.Run();

        private void LoadTexArray()
        {
            TextureManager.TextureArray.Bind();

            TextureManager.TextureArray.SetParameter(TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            TextureManager.TextureArray.SetParameter(TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            TextureManager.TextureArray.SetParameter(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            TextureManager.TextureArray.SetParameter(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            TextureManager.TextureArray.SetParameter(TextureParameterName.TextureLodBias, -1f);

            TextureManager.TextureArray.GenMipMap();
        }

        private unsafe void OnLoad()
        {
            _input = _win.CreateInput();

            _kb = _input.Keyboards[0];

            foreach (var kb in _input.Keyboards)
            {
                kb.KeyDown += OnKeyDown;
            }

            foreach (var mouse in _input.Mice)
            {
                mouse.MouseMove += OnMouseMove;
            }

            GL = GL.GetApi();

            //_glfw = Glfw.GetApi();

            var handle = (WindowHandle*)_win.Handle;

            _glfw.SetInputMode(handle, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);

            TextureManager = new TextureManager(this, _fs);

            LoadTexArray();

            BlockManager = new BlockManager(TextureManager);

            BlockManager.StoreBlock(new BasicBlock
            {
                Name = "dirt",
                TextureFront = "Textures/Blocks/dirt.png"
            });

            BlockManager.StoreBlock(new Block
            {
                Name = "grass",
                TextureFront = "Textures/Blocks/grass_side.png",
                TextureBack = "Textures/Blocks/grass_side.png",
                TextureLeft = "Textures/Blocks/grass_side.png",
                TextureRight = "Textures/Blocks/grass_side.png",
                TextureBottom = "Textures/Blocks/dirt.png",
                TextureTop = "Textures/Blocks/grass_top.png",
            });

            BlockManager.StoreBlock(new BasicBlock
            {
                Name = "stone",
                TextureFront = "Textures/Blocks/stone.png"
            });

            BlockManager.StoreBlock(new BasicBlock
            {
                Name = "sand",
                TextureFront = "Textures/Blocks/sand.png"
            });

            BlockManager.PreloadTextures();

            _shaderManager = new ShaderManager(this, _fs);
            _camera = new Camera(this, new Vector3(0, 18, 0));
            _chunkGen = new ChunkGenerator(this);

            _shader = _shaderManager.CreateProgram("Shaders/Block.vsh", "Shaders/Block.fsh");

            _camera.Fov = 90;

            _chunkGen.Start(_camera);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Src1Alpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.FrontFace(FrontFaceDirection.CW);
        }

        private void OnRender(double delta)
        {
            DeltaTime = delta;

            CheckInput();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.SkyBlue);

            _shader.Use();

            _shader.SetUniform("Projection", _camera.GetProjectionMatrix());
            _shader.SetUniform("View", _camera.GetViewMatrix());

            var camPos = _camera.Position;

            _chunkGen.Render((int)camPos.X, (int)camPos.Z);
        }

        private void CheckInput()
        {
            CameraDirection camDir = 0;

            if (_kb.IsKeyPressed(Key.W))
            {
                camDir |= CameraDirection.Forward;
            }

            if (_kb.IsKeyPressed(Key.A))
            {
                camDir |= CameraDirection.Left;
            }

            if (_kb.IsKeyPressed(Key.S))
            {
                camDir |= CameraDirection.Backward;
            }

            if (_kb.IsKeyPressed(Key.D))
            {
                camDir |= CameraDirection.Right;
            }

            if (_kb.IsKeyPressed(Key.ShiftLeft))
            {
                camDir |= CameraDirection.Boost;
            }

            if (_kb.IsKeyPressed(Key.Space))
            {
                camDir |= CameraDirection.Jump;
            }

            if (_kb.IsKeyPressed(Key.ControlLeft))
            {
                camDir |= CameraDirection.Crouch;
            }

            _camera.Move(camDir);
        }

        private void OnMouseMove(IMouse mouse, PointF point)
        {
            CursorMove?.Invoke(point);
        }

        private void OnKeyDown(IKeyboard kb, Key key, int obj)
        {
            if (key == Key.Escape)
            {
                _win.Close();

                return;
            }

            if (key == Key.F)
            {
                Wireframe = !Wireframe;
            }

            KeyDown?.Invoke(key);
        }

        private void OnClose()
        {
            _chunkGen.Stop();
        }
    }
}