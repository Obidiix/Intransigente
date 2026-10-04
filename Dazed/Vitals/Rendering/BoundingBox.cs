using System.Numerics;
using Dazed.Vitals.Rendering.Debug;

namespace Dazed.Vitals.Rendering;

public class AABB : Volume
{
    // private readonly struct AABBInternal
    // {
    //     public readonly Vector3 Center;
    //     public readonly Vector3 Extents;
    //
    //     public AABBInternal(Vector3 min, Vector3 max)
    //     {
    //         Center = (max + min) * 0.5f;
    //         Extents = Vector3.Abs(max - Center);
    //     }
    //
    //     public AABBInternal(Vector3 center, float iI, float iJ, float iK)
    //     {
    //         Center = center;
    //         Extents = new Vector3(MathF.Max(iI, 0.1f), MathF.Max(iJ, 0.1f), MathF.Max(iK, 0.1f));
    //     }
    // }

    private readonly struct BoundingBox
    {
        public readonly Vector3 Center;
        public readonly Vector3[] Vertices;
        
        public BoundingBox(Vector3 center, Vector3[] vertices)
        {
            Center = center;
            Vertices = vertices;
        }
    }
    
    private static readonly Func<float, float> Abs = MathF.Abs;
    
    // private readonly AABBInternal _aabb;
    private readonly BoundingBox _boundingBox;
    public AABB(Transform? reference, Vector3 min, Vector3 max)
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
        _boundingBox = new BoundingBox((max + min) * 0.5f, vertices);
        // _aabb = new AABBInternal(min, max);
    }
    
    public override bool IsInFrustum(Frustum frustum)
    {
        Vector3 center = Vector3.Transform(_boundingBox.Center, (Matrix4x4)Reference.World);

        // Vector3 right = Reference.Right * _aabb.Extents.X;
        // Vector3 up = Reference.Up * _aabb.Extents.Y;
        // Vector3 forward = Reference.Forward * _aabb.Extents;
        // float newIi = AbsDot(Vector3.UnitX, right) + AbsDot(Vector3.UnitX, up) +
        //               AbsDot(Vector3.UnitX, forward);
        // float newIj = AbsDot(Vector3.UnitY, right) + AbsDot(Vector3.UnitY, up) +
        //               AbsDot(Vector3.UnitY, forward);
        // float newIk = AbsDot(Vector3.UnitZ, right) + AbsDot(Vector3.UnitZ, up) +
        //               AbsDot(Vector3.UnitZ, forward);
        //     
        // var tAabb = new AABBInternal(center, newIi, newIj,  newIk);
        
        Vector3[] vertices = new Vector3[8];
        for (int i = 0; i < 8; i++)
            vertices[i] = Vector3.Transform(_boundingBox.Vertices[i], Reference.World);
        var box = new BoundingBox(center, vertices);
        return IsOnPlane(box, frustum);
    }
    private static float AbsDot(Vector3 vecA, Vector3 vecB) => Abs(Vector3.Dot(vecA, vecB));

    private static bool IsOnPlane(BoundingBox boundingBox, Frustum frustum)
    {
        #if DEBUG
        int nI = 0;
        
        for (int i = 0; i < boundingBox.Vertices.Length; i++)
        {
#if DEBUG
            var box = new TemporalBox();
            box.World = Matrix4x4.CreateScale(new Vector3(0.1f)) * Matrix4x4.CreateTranslation(boundingBox.Vertices[i]);
            box.Color = Color.Red;
            Artist.QueueTemporal(box);
#endif
        }
        #endif
        for (int i = 0; i < Frustum.PlaneQuantity - 1; i++)
        {
            if (IsOnPlane(boundingBox, frustum[i]))
            {
                #if DEBUG
                //Scriptorium.Warn($"On plane: <{i}>, not culled");
                #endif    
                return true;
            }
            #if DEBUG
            nI = i;
            #endif
        }
#if DEBUG
        Scriptorium.Warn("On no plane");
#endif
        return false;
    }
    
    private static bool IsOnPlane(BoundingBox boundingBox, Plane plane)
    {
//         Vector3 extnt = new(boundingBox.Extents.X * Abs(plane.Normal.X), boundingBox.Extents.Y * Abs(plane.Normal.Y), boundingBox.Extents.Z * Abs(plane.Normal.Z));
//         var r = extnt.X + extnt.Y + extnt.Z;
//         var ifZero = boundingBox.Center == Vector3.Zero ? new Vector3(0.1f) : Vector3.Zero;
//         var dot = Vector3.Dot(plane.Normal + ifZero, boundingBox.Center + ifZero) - plane.D;
//         bool result;
//         result = Abs(dot) <= r;
// #if DEBUG
//         var box = new TemporalBox();
//         box.World = Matrix4x4.CreateScale(boundingBox.Extents) * Matrix4x4.CreateTranslation(boundingBox.Center);
//         box.Color = Color.Yellow;
//         Artist.QueueTemporal(box);
//         if(!result)
//             Vector3.Abs(extnt);
// #endif
        for (int i = 0; i < boundingBox.Vertices.Length; i++)
        {
            var mPoint = boundingBox.Vertices[i].LengthSquared();
            var dot = Vector3.Dot(plane.Normal, boundingBox.Vertices[i]) + plane.D;
            if(dot * dot >= mPoint)
                return true;
        }
        return false;
    }
}