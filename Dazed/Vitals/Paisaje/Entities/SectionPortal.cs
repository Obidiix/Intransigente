using System.Numerics;
using Dazed.Vitals.Rendering;

namespace Dazed.Vitals.Paisaje.Entities;
/// <summary>
/// Separates paisaje's sections and allows them to render only if the portal is visible by the camera's view frustum
/// </summary>
[BrushElement("Base/Portal")]
public class SectionPortal : PaisajeDynamic, ICullableObject
{
    [PaisajeField]
    private Vector3 min, max;
    
    public Transform? Transform => _transform;
    private Transform? _transform;

    public bool IsEnabled { get; set; } = true;
    private BoundingBox? _boundingBox;
    
    public bool IsInFrustum(Frustum frustum) => !IsDestroyed && IsEnabled && _boundingBox!.IsInFrustum(frustum);

    protected override void OnCreation()
    {
        _transform = new Transform();
        _boundingBox = new BoundingBox(_transform, min, max);
    }

    protected override void OnDestruction()
    {
        _boundingBox = null;
        _transform = null;
    }
}