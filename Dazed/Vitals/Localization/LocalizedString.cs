using Dazed.Vitals.AssetReference;

namespace Dazed.Vitals.Localization;

public readonly struct LocalizedString : IExposedElement
{
	public readonly string Identifier;
	public readonly int TableIndex;
	
	public bool IsValid => TableIndex >= 0;
	
	public string Text => Locals.RetrieveString(TableIndex);
	
	internal LocalizedString(string identifier, int tableIndex)
	{
		TableIndex = tableIndex;	
		Identifier = identifier;
	}

	/// <summary>
	/// Creates a localized string and finds the proper identifier for it.<br></br>
	/// Recommended only for specific use cases.
	/// </summary>
	/// <param name="identifier"></param>
	public LocalizedString(string identifier)
	{
		Identifier = identifier;
        TableIndex = Locals.AssignTableIndex(Identifier);
	}
	
	public static implicit operator string(LocalizedString localizedString) => localizedString.Text;
}