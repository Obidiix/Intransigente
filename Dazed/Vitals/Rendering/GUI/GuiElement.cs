using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering.GUI
{
    public abstract class GuiElement : Renderable
    {
        public override RenderingPass RenderPass => RenderingPass.userInterface;

        public Vector2 ScreenPosition
        {
            get => _pos;
            set
            {
                _pos = value;
                ReloadWorldMatrix();
            }
        }
        private Vector2 _pos;

        public Vector2 Size
        {
            get => _scl;
            set
            {
                _scl = value;
                ReloadWorldMatrix();
            }
        }
        private Vector2 _scl = Vector2.One;

        protected Matrix4 World => _world;
        protected Matrix4 ScaleMatrix => _scaleMatrix;
        private Matrix4 _world, _scaleMatrix;

        public override bool AllowRendering => _allowRendering;
        protected bool _allowRendering = true;

        public void CanBeRendered(bool state) => _allowRendering = state;

        public GuiElement()
        {
            ReloadWorldMatrix();
            Artist.OnWidowResize += OnFrameBufferResize;
        }

        protected virtual void OnFrameBufferResize() 
        {
            ReloadWorldMatrix();            
        }

        public override void Dispose()
        {
            base.Dispose();
            Artist.OnWidowResize -= OnFrameBufferResize;
        }

        public void SetPositionInPixels(int x, int y)
        {
            x *= 2;
            y *= 2;
            ScreenPosition = new((x * Artist.NormalizedWidth) - 1, (y * Artist.NormalizedHeight) - 1);
        }

        public void SetPositionRelative(float x, float y)
        {
            x *= 2;
            y *= 2;
            ScreenPosition = new(x - 1, y - 1);
        }

        protected virtual void ReloadWorldMatrix()
        {
            _scaleMatrix = Matrix4.CreateScale(_scl.X * 100 * (float)Artist.NormalizedWidth, -_scl.Y * 100 * (float)Artist.NormalizedHeight, 1);
            _world = _scaleMatrix * Matrix4.CreateTranslation(new(_pos.X, -_pos.Y, 0));
        }
    }
}
