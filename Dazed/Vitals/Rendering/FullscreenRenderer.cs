using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal class FullscreenRenderer
    {
        readonly int _vertexBufferObject, _vertexArrayObject, _elementsBufferObject;
        readonly Shader _screenShader;

        ///From https://github.com/opentk/LearnOpenTK/blob/master/Chapter1/9-Camera/Window.cs
        private readonly float[] _vertData =
        {
            // Position         Texture coordinates
             1f,  1f, 0.0f, 1.0f, 1.0f, // top right
             1f, -1f, 0.0f, 1.0f, 0.0f, // bottom right
            -1f, -1f, 0.0f, 0.0f, 0.0f, // bottom left
            -1f,  1f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public FullscreenRenderer(Shader screenShader)
        {
            _screenShader = screenShader;
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertData.Length * sizeof(float), _vertData, BufferUsageHint.StaticDraw);

            _elementsBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementsBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            PositionAttributePointer();
            GL.BindVertexArray(0);
        }

        private void PositionAttributePointer()
        {
            _screenShader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public void Render(int textureId)
        {
            GL.BindVertexArray(_vertexArrayObject);
            _screenShader.Use();
            GL.Disable(EnableCap.DepthTest);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
