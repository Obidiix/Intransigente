namespace Dazed.Vitals.Rendering;

public interface ICullableObject
{
	public bool IsInFrustum(Frustum frustum);
}