namespace Dazed.Vitals.Rendering;

public abstract class Volume
{
    protected Transform Reference { get; init; }
    public abstract bool IsInFrustum(Frustum frustum);
}