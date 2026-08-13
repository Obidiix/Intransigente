using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class Camera : IDisposable
    {
        public bool IsDisposed => _isDisposed;
        private bool _isDisposed;

        public float FieldOfView => _fov;
        private float _fov;

        public Matrix4 View { get; private set; }
        public Matrix4 Projection { get; private set; }
        public Transform Transform;

        private static Action OnProjectionReload;
        public Camera(float fovInDegrees)
        {
            Transform = new();
            _fov = fovInDegrees;
            ReloadProjection();
            OnProjectionReload += ReloadProjection;
        }

        private void ReloadProjection()
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Artist.AspectRatio, 0.3f, 100f);
        }

        public void Move(System.Numerics.Vector3 movement, float factor = 1)
        {
            if (movement == System.Numerics.Vector3.Zero)
                return;
            System.Numerics.Vector3 normalizedMovement = Transform.Forward * movement.Z + Transform.Right * movement.X;
            Transform.Position += System.Numerics.Vector3.Normalize(normalizedMovement) * factor;
        }

        public void PreDrawCall()
        {
            View = Matrix4.LookAt((Vector3)Transform.Position, (Vector3)(Transform.Position - Transform.Forward), Vector3.UnitY);
        }

        public static void ReloadAllProjections() => OnProjectionReload?.Invoke();

        public void Dispose()
        {
            OnProjectionReload -= ReloadProjection;
        }
    }
}
