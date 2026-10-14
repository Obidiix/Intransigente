using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class PointLight : Light
    {
        public override LightCategory Category => LightCategory.Point;
        public Vector3 Position { get; set; }
        public float Radius { get; set; }

        internal override void SetValuesTo(Material material, string structName)
        {
            base.SetValuesTo(material, structName);
            material.SetVector3(structName + "position", Position);
            material.SetFloat(structName + "radius", Radius * 0.01f);
        }

        internal override Half ProcessLightWeight(ViewportCamera camera)
        {
            var weight = Vector3.DistanceSquared(camera.Transform.Position, Position);
            return -(Half)weight;
        }

        internal PointLight(Vector3 position, float radius, float intensity, Color color) : base(intensity, color)
        {
            Position = position;
            Radius = radius;
        }
    }
}
