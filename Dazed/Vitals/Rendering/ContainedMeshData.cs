using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    [Flags]
    public enum ContainedMeshData
    {
        none = 0,
        position = 1,
        normals = 2,
        positionAndNormals = 3,
        textureCoordinates = 4,
        positionAndTextureCoordinates = 5,
        all = 7,
    }
}
