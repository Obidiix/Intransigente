using System.Numerics;

namespace Dazed.Vitals.Rendering
{
    public class Camera : IDisposable
    {
        public static Camera? MainCamera { get; private set; }
        public bool IsMainCamera { get; private set; }
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

        public Matrix4x4 View => _viewMatrix;
        private Matrix4x4 _viewMatrix;
        public Matrix4x4 Projection => _projMatrix;
        private Matrix4x4 _projMatrix;
        public readonly Transform Transform;
        
        public Frustum ViewFrustum { get; private set; }

        public Camera(float fovInDegrees)
        {
            Transform = new Transform();
            _fov = fovInDegrees;
            _nearClipPlane = 0.3f;
            _farClipPlane = 100f;
            ReloadProjection();
            Artist.OnWidowResize += ReloadProjection;
            if (MainCamera == null)
            {
                MainCamera = this;
                IsMainCamera = true;
            }
            Artist.AddCamera(this);
        }

        private void ReloadProjection()
        {
            _projMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathTeacher.DegreesToRadians * _fov, Artist.RenderAspectRatio, _nearClipPlane, _farClipPlane);
            RecalculateViewFrustum();
        }

        private void RecalculateViewFrustum()
        {
            ViewFrustum = new Frustum(_fov, Transform.Position, Transform.Forward, Transform.Right, Transform.Up, Artist.AspectRatio, _nearClipPlane, _farClipPlane);
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
            _viewMatrix = Matrix4x4.CreateLookAt((Vector3)Transform.Position, (Vector3)(Transform.Position - Transform.Forward), Transform.Up);
            //TODO: Should not be executed each frame
            RecalculateViewFrustum();
        }

        public void Dispose()
        {
            if (IsMainCamera)
            {
                if(DebugState.DebugMedium)
                    Scriptorium.Warn("Main camera disposed");
                MainCamera = null;
            }
            Artist.OnWidowResize -= ReloadProjection;
            Artist.RemoveCamera(this);
            GC.SuppressFinalize(this);
        }
    }
}
