using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;

namespace Dazed.Vitals.Paisaje;

public class PaisajeInstance : Renderable
{
    private readonly PaisajeGeometry[] _brushes;
    private readonly List<PaisajeDynamic> _elements = [];
    private readonly string[] _paisajeContext;
    public PaisajeVariables Variables;
    
    public PaisajeInstance(string assetAbsolutePath)
    {
        var sp = Cereal.LoadData<SerializedPaisaje>(assetAbsolutePath);
        _brushes = new PaisajeGeometry[sp.Elements.Length];
        _paisajeContext = sp.Context;
        for (var i = 0; i < sp.Elements.Length; i++)
        {
            switch (sp.Elements[i].PaisajeElementType)
            {
                case PaisajeElementType.Brush:
                    _brushes[i] = new PaisajeGeometry(sp.Elements[i]); 
                    break;
                case PaisajeElementType.DynamicBrush:
                    _elements.Add(InstantiateElement(sp.Elements[i]) ?? throw new NullReferenceException($"Element at index {i} failed on being instantiated"));
                    break;
                case PaisajeElementType.Entity:
                    _elements.Add(InstantiateElement(sp.Elements[i]) ?? throw new NullReferenceException($"Element at index {i} failed on being instantiated"));
                    break;
            }
        }
    }

    private static PaisajeDynamic? InstantiateElement(SerializedPaisajeElement spElement) => PaisajeDynamic.Create(Mundo.GetElementType(spElement.ElementType), spElement.ElementName, spElement.ElementData);

    internal void Destroy()
    {
        foreach (var element in _elements)
            element.Destroy();
    }

    public override bool AllowRendering => true;
    public override void Render(in RenderingData data)
    {
    }
}