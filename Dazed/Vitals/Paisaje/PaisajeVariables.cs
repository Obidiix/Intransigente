using Dazed.Vitals.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Paisaje
{
    public struct PaisajeVariables
    {
        public float AmbientLightIntensity;
        public Color AmbientLightColor;
        public string? SkyboxPath;

        public PaisajeVariables(float ambientLightIntensity, Color ambientLightColor, string? skyboxPath)
        {
            AmbientLightColor = ambientLightColor;
            AmbientLightIntensity = ambientLightIntensity;
            SkyboxPath = skyboxPath;
        }
    }
}
