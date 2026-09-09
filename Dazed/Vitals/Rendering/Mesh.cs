using Assimp;
using Dazed.Vitals.Content;
using OpenTK.Mathematics;
using System.Numerics;

namespace Dazed.Vitals.Rendering
{
    public class Mesh : Renderable
    {
        public MeshPart[] MeshParts { get; private set; }

        public override bool AllowRendering => _allowRendering;
        private bool _allowRendering = true;

        private readonly uint _vertexCount;
        public uint VertexCount => _vertexCount;

        private Matrix4 _world;

        private static readonly Dictionary<string, Mesh> AllMeshInstance = [];
        public void ApplyRelativeToTransform(Matrix4 world) => _world = world;

        public void AllowForRendering(bool value) => _allowRendering = value; 

        public static Mesh GetMeshInstance(string path, Material material, MeshProperties? properties = null, bool pathIsAbsolute = false)
        {
            if (!pathIsAbsolute)
                path = PathUtils.BaseDirectoryCombine(path);
            if (AllMeshInstance.TryGetValue(path, out Mesh m))
                return m;
            MeshProperties props = properties ?? MeshProperties.Default;
            m = new Mesh(path, material, props);
            AllMeshInstance.Add(path, m);
            Artist.AssignRendered(m);
            return m;
        }

        public override void Render(in RenderingData data)
        {
            foreach (MeshPart part in MeshParts)
                part.Render(in data, _world);
        }

        private Mesh(string path, Material material, MeshProperties properties)
        {
            _vertexCount = 0;
            var meshData = Chest.AssimpContext.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.JoinIdenticalVertices);
            List<MeshPart> allParts = new();
            for (int i = 0; i < meshData.Meshes.Count; i++)
            {
                List<float> allVerts = new();
                List<float> allNormals = new();
                List<float> allTextureCoords = new();
                Assimp.Mesh mesh = meshData.Meshes[i];
                _vertexCount += Convert.ToUInt32(mesh.VertexCount);
                for (int v = 0; v < mesh.Vertices.Count; v++)
                {
                    allVerts.Add(mesh.Vertices[v].X);
                    allVerts.Add(mesh.Vertices[v].Y);
                    allVerts.Add(mesh.Vertices[v].Z);
                    if (!properties.UseTextureCoords)
                        continue;
                    allTextureCoords.Add(mesh.TextureCoordinateChannels[0][v].X);
                    allTextureCoords.Add(mesh.TextureCoordinateChannels[0][v].Y);
                }
                if (properties.UseNormals)
                    for (int n = 0; n < mesh.Normals.Count; n++)
                    {
                        allNormals.Add(mesh.Normals[n].X);
                        allNormals.Add(mesh.Normals[n].Y);
                        allNormals.Add(mesh.Normals[n].Z);
                    }
                MeshPart part = new(allVerts.ToArray(), properties.UseNormals ? allNormals.ToArray() : null, properties.UseTextureCoords ? allTextureCoords.ToArray() : null, mesh.GetUnsignedIndices(), mesh.VertexCount, MathTeacher.ToNumerics(meshData.RootNode.Children[i].Transform * meshData.RootNode.Transform), material);
                allParts.Add(part);
            }
            MeshParts = allParts.ToArray();
        }
    }
}
