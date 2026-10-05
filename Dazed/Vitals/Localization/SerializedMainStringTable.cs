using Newtonsoft.Json;

namespace Dazed.Vitals.Localization;

public readonly struct SerializedMainStringTable
{
	[JsonProperty("i")]
	private readonly string[] _identifiers;

	public SerializedMainStringTable(string[] identifiers) => _identifiers = identifiers;

	[JsonIgnore]
	public string[] Identifier => _identifiers;
}