using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering.TextRendering
{
    public class RenderedText : IDisposable
    {
        public Color4 Color
        {
            get => _textColor;
            set => _textColor = value;
        }
        private Color4 _textColor = Color4.White;

        public Vector2 Position
        {
            get => _pos;
            set => _pos = value;
        }
        private Vector2 _pos;

        private Matrix4 _charScale;

        public Font? TextFont
        {
            get => _textFont;
            set => _textFont = value;
        }
        private Font? _textFont;

        private static readonly Shader _textShader;
        public float HorizontalSpacing
        {
            get => _horizontalSpacing;
            set
            {
                _horizontalSpacing = value;
                ReloadText(false);
            }
        }
        private float _horizontalSpacing = 1;

        public float VerticalSpacing
        {
            get => _verticalSpacing;
            set
            {
                _verticalSpacing = value;
                ReloadText(false);
            }
        }
        private float _verticalSpacing = 1;

        public int CharactersPerLine
        {
            get => _charPerLine;
            set
            {
                if (value <= 0)
                    return;
                _charPerLine = value;
                ReloadText(false);
            }
        }
        private int _charPerLine = 10;

        public float Size
        {
            get => _size;
            set
            {
                if (value <= 0)
                    _size = 0.1f;
                else
                    _size = value;
                ReloadText(false);
            }
        }
        private float _size = 12;

        public string Text
        {
            get => _text;
            set
            {
                if (value == null)
                    _text = string.Empty;
                else
                    _text = value;
                ReloadText();
            }
        }
        private string _text = string.Empty;

        private Vector2[] charPositions;
        private char[] charValues;

        private static readonly int _vao, _ebo, _vbo;

        static RenderedText()
        {
            _textShader = new("text");

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, QuadData.VertexPosAndTexturePosData.Length * sizeof(float), QuadData.VertexPosAndTexturePosData, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, QuadData.Indices.Length * sizeof(uint), QuadData.Indices, BufferUsageHint.StaticDraw);

            int positionLocation = _textShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int textureCoordLocation = _textShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(textureCoordLocation);
            GL.VertexAttribPointer(textureCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public RenderedText(Font font)
        {
            _textFont = font;
            Artist.OnWidowResize += ReloadScaleMatrixOnResize;
        }

        private void ReloadText(bool reloadContent = true)
        {
            if (_text == string.Empty)
            {
                charPositions = null;
                charValues = null;
                return;
            }

            TryReloadScaleMatrix();
            ReloadTextPositionAndContent(reloadContent);
        }

        private void ReloadTextPositionAndContent(bool reloadContent)
        {
            charPositions = new Vector2[_text.Length];
            if (reloadContent)
                charValues = new char[_text.Length];
            int offsetX = 0;
            int offsetY = 0;
            int accumulated = 0;
            for (int i = 0; i < _text.Length; i++)
            {
                if (reloadContent)
                    charValues[i] = _text[i];
                int line = (int)MathF.Round(i / _charPerLine);
                if (charValues[i] == '\n')
                {
                    offsetY++;
                    offsetX = i + 1;
                    accumulated = 0;
                    continue;
                }
                if (charValues[i] == ' ')
                    continue;
                if (accumulated != 0 && accumulated % _charPerLine == 0)
                {
                    offsetY++;
                    offsetX = i;
                }
                float x = (i - offsetX) * (_horizontalSpacing * Artist.NormalizedWidth * _size * 10);
                float y = (0 - offsetY * (_verticalSpacing * Artist.NormalizedHeight * _size * 10));
                accumulated++;

                charPositions[i] = new(x, y);
            }
        }

        private float _lastSize;
        private void TryReloadScaleMatrix()
        {
            if (_lastSize == _size)
                return;

            ReloadScaleMatrixShared();
        }

        private void ReloadScaleMatrixOnResize()
        {
            ReloadScaleMatrixShared();
            ReloadTextPositionAndContent(false);
        }

        private void ReloadScaleMatrixShared()
        {
            float scaleX = (_size * 10) * _textFont.TextureCharScale.X;
            float scaleY = (_size * 10) * _textFont.TextureCharScale.Y;
            _charScale = Matrix4.CreateScale(scaleX * Artist.NormalizedWidth, -scaleY * Artist.NormalizedHeight, 1);

            _lastSize = _size;
        }

        internal void Render()
        {
            if (_textFont == null)
            {
                Scriptorium.Warn("Text renderer without font assigned. Skipped.");
                return;
            }

            GL.BindVertexArray(_vao);
            _textShader.Use();
            _textFont._fontTexture.Use();
            _textShader.SetVector2("texCoordsScale", _textFont.TextureCharScale);
            _textShader.SetVector4("textColorMultiplier", _textColor.ToVector4());

            for(int i = 0; i < _text.Length; i++)
            {
                if (_text[i] == ' ')
                    continue;
                if (_text[i] == '\n')
                    continue;
                Matrix4 charMatrix = _charScale * Matrix4.CreateTranslation(charPositions[i].X + _pos.X, charPositions[i].Y + _pos.Y, 0);
                _textShader.SetMatrix4("world", ref charMatrix);
                _textShader.SetVector2("texCoordsShift", _textFont.GetCharPosInAtlas(_text[i]));
                GL.DrawElements(BeginMode.Triangles, QuadData.Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            Artist.OnWidowResize -= ReloadScaleMatrixOnResize;
        }
    }
}
