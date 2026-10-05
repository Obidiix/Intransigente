using System.Numerics;

namespace Dazed.Vitals.Rendering;

public readonly struct Frustum(Matrix4x4 viewProjection)
{
    public readonly Matrix4x4 ViewProjection => viewProjection;
    // public const int PlaneQuantity = 6;
    // public readonly Plane NearFace;
    // public readonly Plane FarFace;
    // public readonly Plane RightFace;
    // public readonly Plane LeftFace;
    // public readonly Plane TopFace;
    // public readonly Plane BottomFace;
    //
    // public Frustum(float horizontalFieldOfView, Vector3 camPos, Vector3 forward, Vector3 right, Vector3 up, float aspectRatio, float zNear, float zFar)
    // {
    //     var halfVSide = zFar * MathF.Tan(horizontalFieldOfView * 0.5f);
    //     var halfHSide = halfVSide * aspectRatio;
    //     
    //     Vector3 frontMultFar = zFar * forward;
    //     
    //     NearFace = new(forward,(camPos + zNear * forward).Length());
    //     FarFace = new(-forward,(camPos + frontMultFar).Length());
    //
    //     var length = camPos.Length();
    //     RightFace = new(Vector3.Normalize(Vector3.Cross(frontMultFar - right * halfHSide, up)),length);
    //     LeftFace = new(Vector3.Normalize(Vector3.Cross(up, frontMultFar + right * halfHSide)),length);
    //     
    //     TopFace = new(Vector3.Normalize(Vector3.Cross(right, frontMultFar - up * halfVSide)),length);
    //     BottomFace = new(Vector3.Normalize(Vector3.Cross(frontMultFar + up * halfVSide, right)),length);
    // }
    //
    // public Plane this[int i]
    // {
    //     get
    //     {
    //         return i switch
    //         {
    //             0 => NearFace,
    //             1 => FarFace,
    //             2 => RightFace,
    //             3 => LeftFace,
    //             4 => TopFace,
    //             5 => BottomFace,
    //             _ => throw new IndexOutOfRangeException()
    //         };
    //     }
    // }
}