using System.Numerics;

namespace Dazed.Vitals.Rendering;

public class SphereVolume : Volume
{
    private readonly struct SphereVolumeInternal(Vector3 center, float radius)
    {
        public readonly Vector3 Center = center;
        public readonly float Radius = radius;
    }

    private readonly SphereVolumeInternal _volume;

    public SphereVolume(Transform reference, Vector3 center, float radius)
    {
        Reference = reference;
        _volume = new SphereVolumeInternal(center, radius);
    }
    
    public override bool IsInFrustum(Frustum frustum)
    {
        Vector3 globalScale = Reference.Scale;
        Vector3 globalCenter = Vector3.Transform(_volume.Center, (Matrix4x4)Reference.World);
        float maxScale = MathF.Max(MathF.Max(globalScale.X, globalScale.Y), globalScale.Z);
        maxScale = _volume.Radius * (maxScale * 0.5f);
        return IsInFrustum(frustum, globalCenter, maxScale);
    }

    private static bool IsInFrustum(Frustum frustum, in Vector3 center, in float radius)
    {
        //TODO: Implement
        return false;
    }
}