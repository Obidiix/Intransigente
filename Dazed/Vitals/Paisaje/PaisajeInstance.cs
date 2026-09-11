using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;

namespace Dazed.Vitals.Paisaje;

public class PaisajeInstance : Renderable
{
    private readonly PaisajeBrush[] _brushes;
    private readonly List<PaisajeElement> _elements = [];
    private readonly string[] _paisajeContext;
    
    public PaisajeInstance(IdentifiedAsset asset)
    {
        var sp = Cereal.LoadData<SerializedPaisaje>(asset.AbsolutePath);
        _brushes = new PaisajeBrush[sp.Elements.Length];
        _paisajeContext = sp.Context;
        for (var i = 0; i < sp.Elements.Length; i++)
        {
            if(sp.Elements[i].ElementIsGeometry)
                _brushes[i] = new PaisajeBrush(sp.Elements[i]);
            else
                _elements.Add(InstantiateElement(sp.Elements[i]) ?? throw new NullReferenceException($"Element {sp.Elements[i]} not found"));
        }
    }

    private static PaisajeElement? InstantiateElement(SerializedPaisajeElement spElement) => PaisajeElement.Create(Mundo.GetElementType(spElement.ElementType), spElement.ElementName, spElement.ElementData);

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