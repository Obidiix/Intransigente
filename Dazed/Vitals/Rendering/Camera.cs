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

        public RenderingPass RenderingPasses { get; private set; }
        public float RenderPriority { get; set; }

        public float FieldOfView => _fov;
        private float _fov;

        public Matrix4 View => _viewMatrix;
        private Matrix4 _viewMatrix;
        public Matrix4 Projection => _projMatrix;
        private Matrix4 _projMatrix;
        public Transform Transform;

        public Camera(float fovInDegrees, RenderingPass renderingPasses = RenderingPass.geometryFirst)
        {
            RenderingPasses = renderingPasses;
            Transform = new();
            _fov = fovInDegrees;
            ReloadProjection();
            Artist.OnWidowResize += ReloadProjection;
            Artist.AddCamera(this);
        }

        private void ReloadProjection()
        {
            _projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Artist.RenderAspectRatio, 0.3f, 100f);
        }

        public void Move(System.Numerics.Vector3 movement, float factor = 1)
        {
            if (movement == System.Numerics.Vector3.Zero)
                return;
            Vector3 normalizedMovement = Transform.Forward * movement.Z + Transform.Right * movement.X;
            Transform.Position += Vector3.Normalize(normalizedMovement) * factor;
        }

        public void PreDrawCall()
        {
            _viewMatrix = Matrix4.LookAt((Vector3)Transform.Position, (Vector3)(Transform.Position - Transform.Forward), Vector3.UnitY);
        }

        public void Dispose()
        {
            Artist.OnWidowResize -= ReloadProjection;
            Artist.RemoveCamera(this);
        }
    }
}
