using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class DirectionalLight(Vector3 direction, Color color, float intensity) : Light(intensity, color)
    {
        public override LightCategory Category => LightCategory.Directional;
        public Vector3 Direction { get; set; } = direction;

        internal override Half ProcessLightWeight(ViewportCamera camera) => Half.MaxValue;

        internal override void SetValuesTo(Material material, string structName)
        {
            base.SetValuesTo(material, structName);
            material.SetVector3(structName + "direction", Direction);
        }
    }
}
