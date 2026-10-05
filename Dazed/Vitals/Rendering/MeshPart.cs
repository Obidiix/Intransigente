using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace Dazed.Vitals.Rendering
{
    public class MeshPart : Renderable
    {
        private readonly uint[] _indices;
        private readonly ContainedMeshData _containedMeshData;
        public Material? PartMaterial
        {
            get => _partMaterial;
            set
            {
                _partMaterial = value;
                ReloadMaterialData();
            }
        }
        private Material? _partMaterial;
        private readonly Matrix4x4 _meshMatrix;
        internal Matrix4x4 WorldMatrix { get; set; }

        private readonly int _vertexArrayObject;

        internal MeshPart(float[] vertPos, float[]? normals, float[]? textureCoordinates, uint[] indices, int vertexCount, Matrix4x4 localMeshMatrix, Material material)
        {
            if (vertPos == null || vertPos.Length == 0)
                throw new ArgumentException("Vertex positions array can NOT be null nor empty.");

            _meshMatrix = localMeshMatrix;
            _containedMeshData = ContainedMeshData.position;
            if(normals != null)
                _containedMeshData = (ContainedMeshData)(int)(ContainedMeshData.normals + (int)_containedMeshData);
            if (textureCoordinates != null)
                _containedMeshData = (ContainedMeshData)(int)(ContainedMeshData.textureCoordinates + (int)_containedMeshData);

            var arrayBufferData = GetProperArray(vertPos, normals, textureCoordinates);

            _indices = indices;

            VertexCount = vertexCount;
            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, arrayBufferData.Length * sizeof(float), arrayBufferData, BufferUsageHint.StaticDraw);

            var elementBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            PartMaterial = material;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        private float[] GetProperArray(float[] vertPos, float[] normals, float[] textureCoordinates)
        {
            switch (_containedMeshData)
            {
                case ContainedMeshData.position:
                    return vertPos;
                case ContainedMeshData.positionAndNormals:
                    return ArrangePosAndNormals(vertPos, normals);
                case ContainedMeshData.positionAndTextureCoordinates:
                    return ArrangePosAndTextureCoordinates(vertPos, textureCoordinates);
                default:
                    return ArrangeAll(vertPos, normals, textureCoordinates);
            }
        }

        private float[] ArrangeAll(float[] vertPos, float[] normals, float[] textureCoordinates)
        {
            int accumulated = 0;
            int offset = 0;

            var array = new float[vertPos.Length + normals.Length + textureCoordinates.Length];
            for (int v = 0; v < vertPos.Length; v++)
            {
                array[v + offset] = vertPos[v];
                accumulated += 1;
                if (accumulated == 3)
                {
                    //3 - As normals are X,Y,Z
                    //+2 - As texture coordinates are X,Y
                    offset += 3 + 2;
                    accumulated = 0;
                }
            }
            offset = 3;
            accumulated = 0;
            for (int n = 0; n < normals.Length; n++)
            {
                array[n + offset] = normals[n];
                accumulated += 1;
                if (accumulated == 3)
                {
                    //3 - As vertex position is X,Y,Z
                    //+2 - As texture coords are X,Y,Z
                    offset += 3 + 2;
                    accumulated = 0;
                }
            }
            offset = 5;
            accumulated = 0;
            for(int t = 0; t < textureCoordinates.Length; t++)
            {
                array[t + offset] = textureCoordinates[t];
                accumulated++;
                if(accumulated == 2)
                {
                    //3 - As vertex position is X,Y,Z
                    //+3 - As normals are X,Y,Z
                    offset += 3 + 3;
                    accumulated = 0;
                }
            }
            return array;
        }

        private float[] ArrangePosAndTextureCoordinates(float[] vertPos, float[] textureCoordinates)
        {
            int accumulated = 0;
            int offset = 0;

            float[] array = new float[vertPos.Length + textureCoordinates.Length];
            for (int v = 0; v < vertPos.Length; v++)
            {
                array[v + offset] = vertPos[v];
                accumulated += 1;
                if (accumulated == 3)
                {
                    //2 - As texture coordinates are X,Y
                    offset += 2;
                    accumulated = 0;
                }
            }
            offset = 2;
            for (int n = 0; n < textureCoordinates.Length; n++)
            {
                array[n + offset] = textureCoordinates[n];
                accumulated += 1;
                if (accumulated == 2)
                {
                    //3 - As vertex position is X,Y,Z
                    offset += 3;
                    accumulated = 0;
                }
            }
            return array;
        }

        private float[] ArrangePosAndNormals(float[] vertPos, float[] normals)
        {
            int accumulated = 0;
            int offset = 0;

            float[] array = new float[vertPos.Length + normals.Length];
            for (int v = 0; v < vertPos.Length; v++)
            {
                array[v + offset] = vertPos[v];
                accumulated += 1;
                if (accumulated == 3)
                {
                    //3 - As normals are X,Y,Z
                    offset += 3;
                    accumulated = 0;
                }
            }
            offset = 3;
            for (int n = 0; n < normals.Length; n++)
            {
                array[n + offset] = normals[n];
                accumulated += 1;
                if (accumulated == 3)
                {
                    //3 - As vertex position is X,Y,Z
                    offset += 3;
                    accumulated = 0;
                }
            }
            return array;
        }

        public override void Render(in RenderingData data)
        {
            if (_partMaterial == null)
            {
                if(DebugState.DebugLow)
                    Scriptorium.Warn("A mesh can not be drawn because it has no shader");
                return;
            }
            SetRenderingValues(in data, WorldMatrix);
            GL.BindVertexArray(_vertexArrayObject);
            _partMaterial.UseMaterial();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private void SetRenderingValues(in RenderingData data, Matrix4x4 meshWorld)
        {
            Artist.MissingTexture.Use();
            if (_partMaterial == null) return;
            _partMaterial.SetMatrix4("view", data.ViewMatrix);
            _partMaterial.SetMatrix4("projection", data.ProjectionMatrix);
            _partMaterial.SetMatrix4("model", _meshMatrix * meshWorld);

            _partMaterial.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
            _partMaterial.SetVector3("lightColor", new Vector3(1f, 1f, 1f));
            _partMaterial.SetVector3("lightPos", new Vector3(1.5f, 0, 3));
            _partMaterial.SetVector3("viewPos", data.CameraPosition);
        }

        private void ReloadMaterialData()
        {
            if (_partMaterial == null)
                return;
            GL.BindVertexArray(_vertexArrayObject);
            switch (_containedMeshData)
            {
                case ContainedMeshData.position:
                    PositionAttributePointer();
                    break;
                case ContainedMeshData.positionAndNormals:
                    PositionAndNormalAttributePointer();
                    break;
                case ContainedMeshData.positionAndTextureCoordinates:
                    PositionAndTextureCoordinatesAttributePointer();
                    break;
                case ContainedMeshData.all:
                    AllAttributePointer();
                    break;
            }
        }

        private void AllAttributePointer()
        {
            int positionLocation = _partMaterial.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            int normalLocation = _partMaterial.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            int textCoord = _partMaterial.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(textCoord);
            GL.VertexAttribPointer(textCoord, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        private void PositionAndTextureCoordinatesAttributePointer()
        {
            int positionLocation = _partMaterial.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int textCoord = _partMaterial.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(textCoord);
            GL.VertexAttribPointer(textCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        private void PositionAndNormalAttributePointer()
        {
            int positionLocation = _partMaterial.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            int normalLocation = _partMaterial.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        private void PositionAttributePointer()
        {
            int positionLocation = _partMaterial.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }
        public int VertexCount { get; }

        public override bool AllowRendering => true;
    }
}
