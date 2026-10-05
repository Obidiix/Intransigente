using Newtonsoft.Json;

namespace Dazed.Vitals.Localization;

[method: JsonConstructor]
public readonly struct SerializedLanguageInfo(string shortName, string formalizedName, string stringTablePath) : IEquatable<SerializedLanguageInfo>
{
	public bool Equals(SerializedLanguageInfo other) => other.GetHashCode() == GetHashCode();
	public override bool Equals(object? obj) => obj is SerializedLanguageInfo other && Equals(other);
	public override int GetHashCode() => formalizedName.GetHashCode() ^ stringTablePath.GetHashCode() ^ shortName.GetHashCode();

	[JsonProperty("n")] 
	public string ShortName => shortName;
	[JsonProperty("f")]
	public string FormalizedName =>  formalizedName;
	[JsonProperty("s")]
	public string StringTablePath => stringTablePath;

	public static bool operator ==(SerializedLanguageInfo a, SerializedLanguageInfo b) => a.Equals(b);
	public static bool operator !=(SerializedLanguageInfo a, SerializedLanguageInfo b) => !a.Equals(b);
}