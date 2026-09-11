using OpenTK.Mathematics;

namespace Dazed.Vitals.Rendering
{
    public class Camera : IDisposable
    {
        public RenderingPass RenderingPasses { get; private set; }
        public float RenderPriority { get; set; }

        public float FieldOfView
        {
            get => _fov;
            set
            {
                _fov = value;
                ReloadProjection();
            }
        }
        private float _fov;

        public float NearClipPlane
        {
            get => _nearClipPlane;
            set
            {
                _nearClipPlane = value;
                ReloadProjection();
            }
        }
        private float _nearClipPlane;
        public float FarClipPlane
        {
            get => _farClipPlane;
            set
            {
                _farClipPlane = value;
                ReloadProjection();
            }
        }
        private float _farClipPlane;

        public Matrix4 View => _viewMatrix;
        private Matrix4 _viewMatrix;
        public Matrix4 Projection => _projMatrix;
        private Matrix4 _projMatrix;
        public readonly Transform Transform;
        
        public Frustum ViewFrustum { get; private set; }

        public Camera(float fovInDegrees, RenderingPass renderingPasses = RenderingPass.geometryFirst)
        {
            RenderingPasses = renderingPasses;
            Transform = new();
            _fov = fovInDegrees;
            _nearClipPlane = 0.3f;
            _farClipPlane = 100f;
            ReloadProjection();
            Artist.OnWidowResize += ReloadProjection;
            Artist.AddCamera(this);
        }

        private void ReloadProjection()
        {
            _projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Artist.RenderAspectRatio, _nearClipPlane, _farClipPlane);
            RecalculateViewFrustum();
        }

        private void RecalculateViewFrustum()
        {
            ViewFrustum = new Frustum(_fov, Transform.Forward, Transform.Up, Transform.Right, Artist.AspectRatio, _nearClipPlane, _farClipPlane);
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
