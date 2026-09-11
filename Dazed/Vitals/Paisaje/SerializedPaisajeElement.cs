using Newtonsoft.Json;

namespace Dazed.Vitals.Paisaje;

public readonly struct SerializedPaisajeElement
{
    [JsonProperty("n")]
    public readonly string ElementName;
    [JsonProperty("t")]
    public readonly string ElementType;
    [JsonProperty("d")]
    public readonly object[] ElementData;
    [JsonProperty("c")]
    public readonly bool ElementIsGeometry;

    [JsonConstructor]
    public SerializedPaisajeElement(string elementName, string elementType, object[] elementData, bool elementIsGeometry)
    {
        ElementName = elementName;
        ElementType = elementType;
        ElementData = elementData;
        ElementIsGeometry = elementIsGeometry;
    }
}