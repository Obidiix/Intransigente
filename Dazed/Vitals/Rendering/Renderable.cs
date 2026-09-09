using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public abstract class Renderable : IDisposable
    {
        public abstract bool AllowRendering { get; }
        public virtual RenderingPass RenderPass => RenderingPass.geometryFirst;
        public abstract void Render(in RenderingData data);

        private readonly int _bindPos;

        public virtual void Dispose()
        {
            Artist.DisposeRendered(_bindPos);
        }

        public Renderable()
        {
            _bindPos = Artist.AssignRendered(this);
        }
    }
}
