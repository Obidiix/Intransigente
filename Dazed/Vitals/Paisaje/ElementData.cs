using Newtonsoft.Json;

namespace Dazed.Vitals.Paisaje;

[method: JsonConstructor]
public readonly struct ElementData(PaisajeFieldData[] fields, object[] data)
{
	[JsonProperty("f")]
	public readonly PaisajeFieldData[] Fields = fields;
	[JsonProperty("d")]
	public readonly object[] Data = data;
}