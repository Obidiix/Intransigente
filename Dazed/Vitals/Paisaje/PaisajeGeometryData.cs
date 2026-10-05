namespace Dazed.Vitals.Paisaje;

public readonly struct PaisajeGeometryData(float[] vertices, uint[] indices)
{
    public readonly float[] Vertices = vertices;
    public readonly uint[] Indices = indices;

    internal static object[] OrganizeGeometryData(PaisajeGeometryData d)
    {
        object[] all = new object[d.Indices.Length + d.Vertices.Length];
        for (int v = 0; v < d.Vertices.Length; v++)
            all[v] = d.Vertices[v];
        for(int i = 0; i < d.Indices.Length; i++)
            all[i + d.Vertices.Length - 1] = d.Indices[i];
        return all;
    }

    internal static PaisajeGeometryData RetrieveGeometryData(ElementData Data)
    {
        var all = Data.Data;
        List<float> vertices = [.. all.TakeWhile(v => v is not uint).Cast<float>()];
        
        var indices = new uint[all.Length - vertices.Count];
        for (var i = 0; i < indices.Length; i++)
            indices[i] = (uint)all[i + vertices.Count - 1];
        return new PaisajeGeometryData(vertices.ToArray(), indices);
    }
}