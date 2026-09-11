using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public readonly struct RenderingData(Matrix4 proj, Matrix4 view, Vector3 camPos, Frustum viewFrustum)
    {
        public readonly Matrix4 ProjectionMatrix => proj;
        public readonly Matrix4 ViewMatrix => view;
        public readonly Vector3 CameraPosition => camPos;
        public readonly Frustum Frustum => viewFrustum;

        //public readonly LightData lightData;
    }
}
