namespace Dazed.Vitals.Rendering
{
    public abstract class Light
    {
        public abstract LightCategory Category { get; }
        public float Intensity { get; set; }
        public Color Color { get; set; }

        internal virtual void SetValuesTo(Material material, string structName)
        {
            material.SetVector3(structName + "color", Color);
            material.SetFloat(structName + "intensity", Intensity * 0.5f);
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
