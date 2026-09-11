namespace Dazed.Vitals.Paisaje;

[AttributeUsage(AttributeTargets.Field)]
public class PaisajeFieldAttribute : Attribute
{
    public bool HasCustomName => CustomFieldName != null;
    public readonly string? CustomFieldName;
    public PaisajeFieldAttribute(){}

    public PaisajeFieldAttribute(string name)
    {
        CustomFieldName = name;
    }
}