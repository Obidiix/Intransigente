using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class MeshPart
    {
        private readonly float[] _arrayBufferData;
        private readonly uint[] _indices;
        private readonly bool _usesNormals;
        public Shader PartShader
        {
            get => _partShader;
            set
            {
                _partShader = value;
                ReloadShaderData();
            }
        }
        private Shader _partShader;
        public Matrix4 LocalMeshMatrix { get; internal set; }

        private readonly int _vertexCount;

        readonly int _vertexBufferObject, _vertexArrayObject, _elementBufferObject;

        public MeshPart(float[] vertices, float[] normals, uint[] indices, int vertexCount, Matrix4 localMeshMatrix, Shader shader)
        {
            LocalMeshMatrix = localMeshMatrix;
            _usesNormals = normals != null;
            if (_usesNormals)
            {
                int accumulated = 0;
                int offset = 0;

                _arrayBufferData = new float[vertices.Length + normals.Length];
                for (int v = 0; v < vertices.Length; v++)
                {
                    _arrayBufferData[v + offset] = vertices[v];
                    accumulated += 1;
                    if (accumulated == 3)
                    {
                        offset += 3;
                        accumulated = 0;
                    }
                }
                offset = 3;
                for (int n = 0; n < normals.Length; n++)
                {
                    _arrayBufferData[n + offset] = normals[n];
                    accumulated += 1;
                    if (accumulated == 3)
                    {
                        offset += 3;
                        accumulated = 0;
                    }
                }
            }
            else
                _arrayBufferData = vertices;

            _indices = indices;

            _vertexCount = vertexCount;
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _arrayBufferData.Length * sizeof(float), _arrayBufferData, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            PartShader = shader;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public void RenderPart()
        {
            if (_partShader == null)
            {
                Scriptorium.Warn("A mesh can not be drawn because it has no shader");
                return;
            }
            GL.BindVertexArray(_vertexArrayObject);
            _partShader.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private void ReloadShaderData()
        {
            if (_partShader == null)
                return;
            GL.BindVertexArray(_vertexArrayObject);
            if (_usesNormals)
                PositionAndNormalAttributePointer();
            else
                PositionAttributePointer();
        }

        private void PositionAndNormalAttributePointer()
        {
            int positionLocation = _partShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            int normalLocation = _partShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        private void PositionAttributePointer()
        {
            int positionLocation = _partShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public int VertexCount => _vertexCount;

    }
}
