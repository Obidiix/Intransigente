using System.Collections.ObjectModel;

namespace Dazed.Vitals.Localization;

public static class Locals
{
	public static SerializedLanguageInfo CurrentLanguage { get; private set; }
	private static readonly ReadOnlyDictionary<string, int> StringsTable;
	private static string[] _stringValues;

	private static readonly Dictionary<string, string> LanguageInfoPaths;
	public static readonly SerializedLanguageInfo[] AvailableLanguages;

	public const string MainStringTablePath = CommonPaths.LocalizationFolderName + "/s.mainTable";
	public const string LocalInfoExtension = ".info";
	public const string StringTableFileExtension = ".st";
	// private static readonly string ImageTableFileExtension = ".it";

	public static event Action? OnLanguageChange;
	
	static Locals()
	{
		LanguageInfoPaths = new Dictionary<string, string>();

		AssignMainStringTable(out StringsTable);

		RetrieveTablesPath(out LanguageInfoPaths, out AvailableLanguages);
		//TODO: make the player choose their preferred language
		CurrentLanguage = AvailableLanguages[0];
		UpdateValues();
	}

	private static void AssignMainStringTable(out ReadOnlyDictionary<string, int> stringTable)
	{
		var path = PathUtils.BaseDirectoryCombine(MainStringTablePath);
		var mainTable = Cereal.LoadData<SerializedMainStringTable>(path);

		var dictionary = new Dictionary<string, int>();

		for (var i = 0; i < mainTable.Identifier.Length; i++)
			dictionary.Add(mainTable.Identifier[i], i);
		stringTable = new(dictionary);
	}

	private static void UpdateValues()
	{
		var stringTablePath = PathUtils.BaseDirectoryCombine(Path.Combine(CommonPaths.LocalizationFolderName, CurrentLanguage.StringTablePath));
		var stringTable = Cereal.LoadData<SerializedStringTable>(stringTablePath);
		_stringValues = new string[stringTable.Content.Length];
		for(int i = 0; i < stringTable.Content.Length; i++)
			_stringValues[i] = stringTable.Content[i];
	}

	private static void RetrieveTablesPath(out Dictionary<string, string> languageInfoPaths, out SerializedLanguageInfo[] availableLanguages)
	{
		var langInfoPaths = new Dictionary<string, string>();
		List<string> filesPaths = [];
		DirectoryInfo directoryInfo = new(PathUtils.BaseDirectoryCombine(CommonPaths.LocalizationFolderName));
		if (!directoryInfo.Exists)
			throw new DirectoryNotFoundException($"Directory {directoryInfo.FullName} does not exist");
		filesPaths.AddRange(from file in directoryInfo.GetFiles() where file.Name.EndsWith(LocalInfoExtension) select file.FullName);

		var info = new SerializedLanguageInfo[filesPaths.Count];
		for (var i = 0; i < filesPaths.Count; i++)
		{
			var languageInfo = Cereal.LoadData<SerializedLanguageInfo>(filesPaths[i]);
			info[i] = languageInfo;
			langInfoPaths.Add(languageInfo.ShortName, filesPaths[i]);
		}
		languageInfoPaths = langInfoPaths;
		availableLanguages = info;
	}

	public static void SetLanguage(SerializedLanguageInfo info)
	{
		CurrentLanguage = info;
		UpdateValues();
		OnLanguageChange?.Invoke();
	}
	
	public static LocalizedString GetLocalizedString(string identifier)
	{
		#if DEBUG
		try
		{
			return new LocalizedString(identifier, StringsTable[identifier]);
		}
		catch (Exception e)
		{
			Scriptorium.LogException(e);
			return new LocalizedString("DEBUG_INVALID", -1);
		}
		#else
		return new LocalizedString(identifier, StringsTable[identifier]);
		#endif
	}

	internal static string RetrieveString(int tableIndex)
	{
		if(tableIndex < 0)
			return $"INVALID_{tableIndex}";
		try
		{
			return _stringValues[tableIndex];
		}
		catch
		{
			var message = $"Localized string not found for index: {tableIndex}";
			Scriptorium.Error(message);
			if(!DebugState.DebugLow)
				throw new IndexOutOfRangeException(message);
			return $"NOTFOUND_{tableIndex}";
		}
	}
}