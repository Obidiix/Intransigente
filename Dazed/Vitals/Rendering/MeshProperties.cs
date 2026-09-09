using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public readonly struct MeshProperties
    {
        public readonly bool UseNormals;
        public readonly bool UseTextureCoords;

        public static readonly MeshProperties Default = new(true, true);
        public static readonly MeshProperties OnlyNormals = new(true, false);
        public static readonly MeshProperties OnlyTextureCoordinates = new(false, true);

        public MeshProperties(bool useNormals, bool useTextureCoords)
        {
            UseNormals = useNormals;
            UseTextureCoords = useTextureCoords;
        }
    }
}
