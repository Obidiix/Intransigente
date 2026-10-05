namespace Dazed.Vitals.Localization;

public readonly struct LocalizedString
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
	
	public static implicit operator string(LocalizedString localizedString) => localizedString.Text;
}