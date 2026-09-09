using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal class CameraPriorityComparer : Comparer<Camera>
    {
        public override int Compare(Camera? x, Camera? y)
        {
            if (x == null || y == null)
                return 0;
            float result = (x.RenderPriority - y.RenderPriority) * 100;
            //Not allowing both to be in the same position
            if (result == 0)
                result = 1;
            return (int)result;
        }
    }
}
