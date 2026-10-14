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

        internal static readonly string[] Parameters = ["position", "direction", "cutOff", "outerCutOff"];

        public SpotLight(Vector3 position, Color color, Vector3 direction, float intensity = 1, float cutOffRadius = MathTeacher.DegreesToRadians * 15, float outerCutOffRadius = MathTeacher.DegreesToRadians * 30) : base(intensity, color)
        {
            Position = position;
            Direction = direction;
            CutOffRadius = cutOffRadius;
            OuterCutOffRadius = outerCutOffRadius;
        }

        internal override void SetValuesTo(Material material, LightParameters parameters)
        {
            base.SetValuesTo(material, parameters);
            material.SetVector3(parameters.GetIdentification(Parameters[0]), Position);
            material.SetVector3(parameters.GetIdentification(Parameters[1]), Direction);
            material.SetFloat(parameters.GetIdentification(Parameters[2]), MathF.Cos(CutOffRadius));
            material.SetFloat(parameters.GetIdentification(Parameters[3]), MathF.Cos(OuterCutOffRadius));
        }

        internal override Half ProcessLightWeight(ViewportCamera camera)
        {
            var weight = Vector3.DistanceSquared(camera.Transform.Position, Position) + (CutOffRadius * CutOffRadius);
            return -(Half)weight;
        }
    }
}
