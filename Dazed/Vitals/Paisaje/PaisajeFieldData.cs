using Newtonsoft.Json;

namespace Dazed.Vitals.Paisaje;

[method:JsonConstructor]
public struct PaisajeFieldData(string fieldName, int objectFieldIndex)
{
	[JsonProperty("f")]
	public readonly string FieldName =>  fieldName;
	[JsonProperty("i")]
	public readonly int ObjectFieldIndex => objectFieldIndex;
}