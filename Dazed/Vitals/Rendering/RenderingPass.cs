using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    [Flags]
    public enum RenderingPass
    {
        geometryFirst = 1,
        geometrySecond = 2,
        geometry = 3,
        userInterface = 4,
    }
}
