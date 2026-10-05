using System.Numerics;

namespace Dazed.Vitals.Rendering;

public class BoundingBox : Volume
{
    private readonly struct BoundingBoxInternal
    {
        public readonly Vector4[] Vertices;
        
        public BoundingBoxInternal(Vector3[] vertices)
        {
            Vertices = new Vector4[vertices.Length];
            for(int i = 0; i < vertices.Length; i++)
                Vertices[i] = new Vector4(vertices[i].X, vertices[i].Y, vertices[i].Z, 1);
        }
    }
    
    private readonly BoundingBoxInternal _boundingBox;
    public BoundingBox(Transform reference, Vector3 min, Vector3 max)
    {
        Reference = reference;
        Vector3[] vertices = new Vector3[8];
        vertices[0] = min;
        vertices[1] = new(min.X, min.Y, max.Z);
        vertices[2] = new(min.X, max.Y, min.Z);
        vertices[3] = new(max.X, min.Y, min.Z);
        vertices[4] = new(max.X, max.Y, min.Z);
        vertices[5] = new(min.X, max.Y, max.Z);
        vertices[6] = new(max.X, min.Y, max.Z);
        vertices[7] = max;
        _boundingBox = new BoundingBoxInternal(vertices);
    }
    
    public override bool IsInFrustum(Frustum frustum) => IsInFrustum(_boundingBox,  Reference.World * frustum.ViewProjection);

    private static bool IsInFrustum(BoundingBoxInternal boundingBox, Matrix4x4 modelViewProjection)
    {
        bool inside = false;
        foreach (var vertex in boundingBox.Vertices)
        {
            var corner = Vector4.Transform(vertex, modelViewProjection);

            inside = inside || MathTeacher.Within(-corner.W, corner.X, corner.W) &&
                MathTeacher.Within(-corner.W, corner.Y, corner.W) &&
                MathTeacher.Within(0, corner.Z, corner.W);
        }
        return inside;
    }
}