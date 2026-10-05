using System.Numerics;

namespace Dazed.Vitals.Rendering
{
    public readonly struct RenderingData(Matrix4x4 proj, Matrix4x4 view, Vector3 camPos, Frustum viewFrustum)
    {
        public readonly Matrix4x4 ProjectionMatrix => proj;
        public readonly Matrix4x4 ViewMatrix => view;
        public readonly Vector3 CameraPosition => camPos;
        public readonly Frustum Frustum => viewFrustum;

        //public readonly LightData lightData;
    }
}
