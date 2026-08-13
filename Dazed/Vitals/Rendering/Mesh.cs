using Assimp;
using Dazed.Vitals.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class Mesh
    {
        public MeshPart[] MeshParts { get; private set; }
        private readonly uint VertexCount;
        public Mesh(string path, Shader shader, bool pathIsAbsolute = false)
        {
            VertexCount = 0;
            if (!pathIsAbsolute)
                path = Path.Combine(PathUtils.ResourcesDirectory, path);
            var meshData = Chest.AssimpContext.ImportFile(path, Assimp.PostProcessSteps.Triangulate | PostProcessSteps.JoinIdenticalVertices);
            List<MeshPart> allParts = new();
            for (int i = 0; i < meshData.Meshes.Count; i++)
            {
                List<float> allVerts = new();
                List<float> allNormals = new();
                Assimp.Mesh mesh = meshData.Meshes[i];
                VertexCount += Convert.ToUInt32(mesh.VertexCount);
                for (int v = 0; v < mesh.Vertices.Count; v++)
                {
                    allVerts.Add(mesh.Vertices[v].X);
                    allVerts.Add(mesh.Vertices[v].Y);
                    allVerts.Add(mesh.Vertices[v].Z);
                }
                for (int n = 0; n < mesh.Normals.Count; n++)
                {
                    allNormals.Add(mesh.Normals[n].X);
                    allNormals.Add(mesh.Normals[n].Y);
                    allNormals.Add(mesh.Normals[n].Z);
                }
                MeshPart part = new(allVerts.ToArray(), allNormals.ToArray(), mesh.GetUnsignedIndices(), mesh.VertexCount, MathTeacher.ToNumerics(meshData.RootNode.Children[i].Transform * meshData.RootNode.Transform), shader);
                allParts.Add(part);
            }
            MeshParts = allParts.ToArray();
        }
    }
}
