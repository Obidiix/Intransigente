using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal class SpotLight : Light
    {
        public override LightCategory Category => LightCategory.SpotLight;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float CutOffRadius { get; set; }
        public float OuterCutOffRadius { get; set; }

        public SpotLight(Vector3 position, Color color, Vector3 direction, float intensity = 1, float cutOffRadius = MathTeacher.DegreesToRadians * 35, float outerCutOffRadius = MathTeacher.DegreesToRadians * 45) : base(intensity, color)
        {
            Position = position;
            Direction = direction;
            CutOffRadius = cutOffRadius;
            OuterCutOffRadius = outerCutOffRadius;
        }

        internal override void SetValuesTo(Material material, string structName)
        {
            base.SetValuesTo(material, structName);
            material.SetVector3("position", Position);
            material.SetVector3("direction", Direction);
            material.SetFloat("cutOffRadius", CutOffRadius);
            material.SetFloat("outerCutOffRadius", OuterCutOffRadius);
        }

        internal override Half ProcessLightWeight(ViewportCamera camera)
        {
            var weight = Vector3.DistanceSquared(camera.Transform.Position, Position) + (CutOffRadius * CutOffRadius);
            return -(Half)weight;
        }
    }
}
