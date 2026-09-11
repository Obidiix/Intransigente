using Dazed.Vitals.Rendering;

namespace Dazed.Vitals.Paisaje;
/// <summary>
/// The building block for the geometry of the paisaje
/// </summary>
public sealed class PaisajeBrush
{
    private readonly string _name;
    public readonly float[] Vertices;
    public readonly uint[] Indices;

    public PaisajeBrush(SerializedPaisajeElement sb)
    {
        Mundo.RetrieveGeometryData(sb, out Vertices, out Indices);
        _name = sb.ElementType;
    }
    
    public override string ToString()
    {
        return $"{base.ToString()} of name: {_name}";
    }
}