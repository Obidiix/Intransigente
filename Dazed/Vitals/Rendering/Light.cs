using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public abstract class Light
    {
        public abstract Color4 Color { get; }
        public abstract Vector3 Position { get; }
        internal void Dispose()
        {

        }
    }
}
