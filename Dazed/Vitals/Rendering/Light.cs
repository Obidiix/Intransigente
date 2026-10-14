using System.Collections;
using System.Runtime.CompilerServices;

namespace Dazed.Vitals.Rendering
{
    public abstract class Light
    {
        public abstract LightCategory Category { get; }
        public float Intensity { get; set; }
        public Color Color { get; set; }
        internal static readonly string[] BaseParameters = ["color", "intensity"];

        internal virtual void SetValuesTo(Material material, LightParameters parameters)
        {
            material.SetVector3(parameters.GetIdentification(BaseParameters[0]), Color);
            material.SetFloat(parameters.GetIdentification(BaseParameters[1]), Intensity * 0.5f);
        }

        internal abstract Half ProcessLightWeight(ViewportCamera camera);

        public Light(float intensity, Color color)
        {
            Intensity = intensity;
            Color = color;
            Artist.RegisterNewLight(this);
        }
    }
}
