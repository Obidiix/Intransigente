using System.Numerics;

namespace Dazed.Vitals.Rendering
{
    public class PointLight : Light
    {
        public override LightCategory Category => LightCategory.Point;
        public Vector3 Position { get; set; }
        public float Radius { get; set; }

        internal static readonly string[] Parameters = ["position", "radius"];

        internal override void SetValuesTo(Material material, LightParameters parameters)
        {
            base.SetValuesTo(material, parameters);
            material.SetVector3(parameters.GetIdentification(Parameters[0]), Position);
            material.SetFloat(parameters.GetIdentification(Parameters[1]), Radius * 0.01f);
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
