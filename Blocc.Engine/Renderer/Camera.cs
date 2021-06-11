using System;
using System.Drawing;
using System.Numerics;
using Blocc.Engine.Common;
using Blocc.Engine.Common.Extensions;

namespace Blocc.Engine.Renderer
{
    [Flags]
    public enum CameraDirection
    {
        Forward = 0x01,
        Left = 0x02,
        Backward = 0x04,
        Right = 0x08,
        Jump = 0x10,
        Crouch = 0x20,
        Boost = 0x40,
    }

    public class Camera
    {
        private const float DefaultYaw = -90;
        private const float DefaultPitch = 0;

        private readonly IGame _game;

        private Vector3 _front;
        private Vector3 _right;
        private Vector3 _up;
        private Vector3 _worldUp;
        private Vector3 _worldFront;

        private float _yaw;
        private float _pitch;

        private float _speed;

        private PointF _mouseLast;
        private bool _mouseCaptured;

        public Vector3 Position { get; private set; }
        public float Fov { get; set; }
        public float Sensitivity { get; set; }

        public Camera(IGame game, float yaw = DefaultYaw, float pitch = DefaultPitch)
            : this(game, Vector3.Zero, Vector3.UnitY, yaw, pitch)
        {
        }

        public Camera(IGame game, Vector3 position, float yaw = DefaultYaw, float pitch = DefaultPitch)
            : this(game, position, Vector3.UnitY, yaw, pitch)
        {
        }

        public Camera(IGame game, Vector3 position, Vector3 up, float yaw = DefaultYaw, float pitch = DefaultPitch)
            : this(game, position, up, new Vector3(0, 0, -1), yaw, pitch)
        {
        }

        public Camera(
            IGame game, Vector3 position, Vector3 up, Vector3 front, float yaw = DefaultYaw,float pitch = DefaultPitch)
        {
            _game = game;

            _front = new Vector3(0, 0, -1);
            _worldUp = up;
            _worldFront = front;

            _yaw = yaw;
            _pitch = pitch;

            Sensitivity = 0.05f;
            _speed = 5;
            Fov = 45;

            Position = position;

            _game.CursorMove += OnCursorMove;

            var options = _game.Options;
        }

        public void Move(CameraDirection direction, float multiplier = 2)
        {
            var velocity = _speed * _game.DeltaTimeF;

            if (direction.HasFlag(CameraDirection.Boost))
            {
                velocity *= multiplier + 1;
            }

            var isForward = direction.HasFlag(CameraDirection.Forward);
            var isBackward = direction.HasFlag(CameraDirection.Backward);

            var isLeft = direction.HasFlag(CameraDirection.Left);
            var isRight = direction.HasFlag(CameraDirection.Right);

            if ((isForward || isBackward) && (isLeft || isRight))
            {
                velocity /= 1.5f;
            }

            if (!(isForward && isBackward))
            {
                if (isForward)
                {
                    Position += _worldFront * velocity;
                }

                if (isBackward)
                {
                    Position -= _worldFront * velocity;
                }
            }

            if (!(isLeft && isRight))
            {
                if (isLeft)
                {
                    Position -= _right * velocity;
                }

                if (isRight)
                {
                    Position += _right * velocity;
                }
            }

            var isJump = direction.HasFlag(CameraDirection.Jump);
            var isCrouch = direction.HasFlag(CameraDirection.Crouch);

            if (!(isJump && isCrouch))
            {
                if (isJump)
                {
                    Position += _worldUp * velocity;
                }

                if (isCrouch)
                {
                    Position -= _worldUp * velocity;
                }
            }
        }
        private void OnCursorMove(PointF point)
        {
            if (!_mouseCaptured)
            {
                _mouseLast = point;

                _mouseCaptured = true;
            }

            var offsetX = point.X - _mouseLast.X;
            var offsetY = _mouseLast.Y - point.Y;

            _mouseLast = point;

            offsetX *= Sensitivity;
            offsetY *= Sensitivity;

            _yaw += offsetX;
            _pitch += offsetY;

            if (_pitch > 89)
            {
                _pitch = 89;
            }

            if (_pitch < -89)
            {
                _pitch = -89;
            }

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            var direction = new Vector3
            {
                X = MathF.Cos(UnitUtils.ToRadiansF(_yaw)) * MathF.Cos(UnitUtils.ToRadiansF(_pitch)),
                Y = MathF.Sin(UnitUtils.ToRadiansF(_pitch)),
                Z = MathF.Sin(UnitUtils.ToRadiansF(_yaw)) * MathF.Cos(UnitUtils.ToRadiansF(_pitch)),
            };

            var worldDir = new Vector3
            {
                X = MathF.Cos(UnitUtils.ToRadiansF(_yaw)),
                Y = 0,
                Z = MathF.Sin(UnitUtils.ToRadiansF(_yaw)),
            };

            _worldFront = Vector3.Normalize(worldDir);

            _front = Vector3.Normalize(direction);
            _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public Matrix4x4 GetProjectionMatrix(float near = 0.01f, float far = 2000)
            => Matrix4x4.CreatePerspectiveFieldOfView(
                UnitUtils.ToRadiansF(Fov), _game.ScreenSize.GetAspectRatio(), near, far);

        public Matrix4x4 GetViewMatrix()
            => Matrix4x4.CreateLookAt(Position, Position  + _front, _up);

        public Matrix4x4 GetProjectionView(float near = 0.1f, float far = 100)
            => GetProjectionMatrix(near, far) * GetViewMatrix();
    }
}