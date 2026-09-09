using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering.GUI
{
    internal class ButtonGUI : ImageGui
    {
        public event Action OnButtonPress, OnButtonRelease, OnButtonHover, OnButtonLostHover;
        public bool IsPressed => _isPressed;
        public bool IsHovered => _isHovered;
        private bool _isPressed, _isHovered;

        private float X0;
        private float Y0;
        private float X1;
        private float Y1;

        public ButtonGUI(Texture2D? tex, Shader? shader = null) : base(tex, shader)
        {
            ReloadData();
        }

        public override void Render(in RenderingData data)
        {
            base.Render(data);
            CheckPress();
        }

        protected override void ReloadWorldMatrix()
        {
            base.ReloadWorldMatrix();
            ReloadData();
        }

        protected override void OnFrameBufferResize()
        {
            base.OnFrameBufferResize();
            ReloadData();
        }

        private void ReloadData()
        {
            float width = Size.X * 50;
            float height = Size.Y * 50;
            float posX = 0.5f * ((ScreenPosition.X + 1) * Artist.Width);
            float posY = 0.5f * ((ScreenPosition.Y + 1) * Artist.Height);

            X0 = (posX - width);
            Y0 = (posY - height);
            X1 = (posX + width);
            Y1 = (posY + height);
        }

        private void CheckPress()
        {
            Vector2 mousePos = Input.MousePosition;
            if (Input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftAlt))
            {
                //SetPositionInPixels((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
                //Scriptorium.Warn(mousePos);
                SetPositionRelative(Input.RelativeMousePosition.X, Input.RelativeMousePosition.Y);
                Scriptorium.Warn(Input.RelativeMousePosition);
            }
            if (mousePos.X > X1 || mousePos.X < X0 || mousePos.Y > Y1 || mousePos.Y < Y0)
            {
                if (_isHovered)
                    OnButtonLostHover?.Invoke();
                _isHovered = false;
                return;
            }
            if (!_isHovered)
                OnButtonHover?.Invoke();
            _isHovered = true;

            if (!Input.LeftClickDown())
            {
                if (_isPressed)
                    OnButtonRelease?.Invoke();
                _isPressed = false;
                return;
            }
            if (_isPressed)
                return;
            Scriptorium.Warn("click");
            _isPressed = true;
            OnButtonPress?.Invoke();
        }
    }
}
