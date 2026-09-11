using System.Numerics;

namespace Dazed.Vitals.Rendering;

public readonly struct Frustum
{
    public readonly Plane NearFace;
    public readonly Plane FarFace;
    public readonly Plane RightFace;
    public readonly Plane LeftFace;
    public readonly Plane TopFace;
    public readonly Plane BottomFace;

    public Frustum(float horizontalFieldOfView, OpenTK.Mathematics.Vector3 forward, OpenTK.Mathematics.Vector3 up, OpenTK.Mathematics.Vector3 right, float aspectRatio, float zNear, float zFar)
    {
        var halfVSide = zFar * MathF.Tan(horizontalFieldOfView * 0.5f);
        var halfHSide = halfVSide * aspectRatio;
        
        var frwd = (Vector3)forward;
        var u = (Vector3)up;
        var rhgt = (Vector3)right;
        
        Vector3 frontMultFar = zFar * frwd;
        
        NearFace = new(zNear * frwd, frwd.Length());
        FarFace = new(frontMultFar, -frwd.Length());
        
        RightFace = new(Vector3.Zero, Vector3.Cross(frontMultFar - rhgt * halfHSide, u).Length());
        LeftFace = new(Vector3.Zero, Vector3.Cross(u, frontMultFar + rhgt * halfHSide).Length());
        
        TopFace = new(Vector3.Zero, Vector3.Cross(rhgt, frontMultFar - u * halfVSide).Length());
        BottomFace = new(Vector3.Zero, Vector3.Cross(frontMultFar + u * halfVSide, rhgt).Length());
    }
}