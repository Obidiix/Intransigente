using System.Numerics;

namespace Dazed.Vitals.Rendering
{
    public class DirectionalLight(Vector3 direction, Color color, float intensity) : Light(intensity, color)
    {
        public override LightCategory Category => LightCategory.Directional;
        public Vector3 Direction { get; set; } = direction;

        internal static readonly string[] Parameters = ["direction"];

        internal override Half ProcessLightWeight(ViewportCamera camera) => Half.MaxValue;

        internal override void SetValuesTo(Material material, LightParameters parameters)
        {
            base.SetValuesTo(material, parameters);
            material.SetVector3(parameters.GetIdentification(Parameters[0]), Direction);
        }
    }
}
