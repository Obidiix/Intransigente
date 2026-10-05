using Newtonsoft.Json;

namespace Dazed.Vitals.Localization;

public readonly struct SerializedStringTable
{
	[JsonProperty("c")]
	private readonly string[] _content;

	public SerializedStringTable(string[] content)
	{
		_content = content;
	}
	
	[JsonIgnore]
	public string[] Content => _content;
}