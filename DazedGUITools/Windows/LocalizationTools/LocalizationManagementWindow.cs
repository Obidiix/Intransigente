using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dazed.Vitals.Localization;
using ImGuiNET;
using Newtonsoft.Json;

namespace DazedGUITools.Windows.LocalizationTools;

[MainMenuWindow("Localization Manager")]
public class LocalizationManagementWindow : WindowBaseClass
{
	private SerializedMainStringTable? _mainStringTable;
	private readonly Dictionary<string, string> LocalizationsPath = new();
	private string[] _availableLocalizations;
	private SerializedLanguageInfo[] _languageInfo;
	private bool LocalizationsExist => _availableLocalizations.Length > 0;
	private static string MainStringTablePath => MainWindow.ProjectFolderCombine(Dazed.Vitals.Localization.Localization.MainStringTablePath);
	private static string LocalizationFolderPath => MainWindow.ProjectFolderCombine(Dazed.Vitals.Localization.Localization.LocalizationFolderName);
	
	public LocalizationManagementWindow() : base("Localization Manager", true)
	{
		if (!Path.Exists(MainStringTablePath))
			return;
		LoadMainStringTable();
		FindLocalizations();
	}

	private void FindLocalizations()
	{
		List<string> filesPaths = [];
		DirectoryInfo directoryInfo = new(LocalizationFolderPath);
		if (!directoryInfo.Exists)
			throw new DirectoryNotFoundException($"Directory {directoryInfo.FullName} does not exist");
		filesPaths.AddRange(from file in directoryInfo.GetFiles() where file.Name.EndsWith(Dazed.Vitals.Localization.Localization.StringTableFileExtension) select file.FullName);
		
		LocalizationsPath.Clear();
		var info = new SerializedLanguageInfo[filesPaths.Count];
		for (var i = 0; i < filesPaths.Count; i++)
		{
			var content = File.ReadAllText(filesPaths[i]);
			var languageInfo = JsonConvert.DeserializeObject<SerializedLanguageInfo>(content);
			info[i] = languageInfo;
			LocalizationsPath.Add(languageInfo.ShortName, filesPaths[i]);
		}
		_availableLocalizations = [.. LocalizationsPath.Keys];
		_languageInfo = info;
	}

	private void LoadMainStringTable()
	{
		var path = MainStringTablePath;
		var content = File.ReadAllText(path);
		_mainStringTable = JsonConvert.DeserializeObject<SerializedMainStringTable>(content);
	}

	private string localIdentifier = "";
	private string localFormalName = "";
	
	protected override void Draw()
	{
		ImGui.Text("Localization Manager");
		ImGui.Spacing();

		if (_mainStringTable == null)
		{
			ShowTableCreation();
			return;
		}

		if (!LocalizationsExist)
		{
			ImGui.Text("No Localizations found");
			ImGui.Spacing();
			
			ImGui.InputText("Local identification", ref localIdentifier, 128);
			ImGui.InputText("Local formal name", ref localFormalName, 128);

			var canCreate = localIdentifier == "" ||  localFormalName == "";
			if(canCreate)
				ImGui.TextColored(Vector4Colors.Red, "Please enter valid values");
			if (ImGui.Button("Create Localization Table") && canCreate)
			{
				using var f = File.Create(Path.Combine(LocalizationFolderPath, $"{localIdentifier}.{LocalizationTools}"));
			}
		}
	}

	private void ShowTableCreation()
	{
		if (ImGui.Button("Create Localization Table"))
		{
			using var f = File.Create(MainStringTablePath);
			_mainStringTable = new SerializedMainStringTable();
		}
	}
}