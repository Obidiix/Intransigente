using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dazed.Vitals;
using Dazed.Vitals.Localization;
using ImGuiNET;
using Newtonsoft.Json;

namespace DazedGUITools.Windows.LocalizationTools;

[MainMenuWindow("Localization Manager")]
public class LocalizationManagementWindow : WindowBaseClass
{
	private bool _mainStringTableAssigned;
	private SerializedMainStringTable _mainStringTable;
	private readonly Dictionary<string, string> _localizationsPath = new();
	private string[]? _availableLocalizations;
	private SerializedLanguageInfo[] _languageInfo;

	private readonly List<string> _identifiers = new();
	private readonly List<string> _entries = new();
	private int _currentLanguageIndex = 0;
	private bool _creatingLocal;
	
	private SerializedLanguageInfo CurrentLanguageInfo => _languageInfo[_currentLanguageIndex];
	private string CurrentStringTablePath => Path.Combine(LocalizationFolderPath, $"{CurrentLanguageInfo.ShortName}{Locals.StringTableFileExtension}");
	
	private bool LocalizationsExist => _availableLocalizations != null && _availableLocalizations.Length > 0;
	private static string MainStringTablePath => MainWindow.ProjectFolderCombine(Locals.MainStringTablePath);
	private static string LocalizationFolderPath => MainWindow.ProjectFolderCombine(CommonPaths.LocalizationFolderName);
	
	private bool CanSave => File.Exists(MainStringTablePath) && File.Exists(CurrentStringTablePath);
	
	public LocalizationManagementWindow() : base("Localization Manager", true)
	{
		if (!Path.Exists(MainStringTablePath))
			return;
		LoadMainStringTable();
		FindLocalizations();
		if (LocalizationsExist)
		{
			_currentLanguageIndex = 0;
			LoadLocalization();
		}
	}

	private void FindLocalizations()
	{
		List<string> filesPaths = [];
		DirectoryInfo directoryInfo = new(LocalizationFolderPath);
		filesPaths.AddRange(from file in directoryInfo.GetFiles() where file.Name.EndsWith(Locals.LocalInfoExtension) select file.FullName);
		
		_localizationsPath.Clear();
		var info = new SerializedLanguageInfo[filesPaths.Count];
		for (var i = 0; i < filesPaths.Count; i++)
		{
			var content = File.ReadAllText(filesPaths[i]);
			var languageInfo = JsonConvert.DeserializeObject<SerializedLanguageInfo>(content);
			info[i] = languageInfo;
			_localizationsPath.Add(languageInfo.ShortName, filesPaths[i]);
		}
		_availableLocalizations = [.. _localizationsPath.Keys];
		_languageInfo = info;
	}

	private void LoadLocalization()
	{
		var path = Path.Combine(LocalizationFolderPath, $"{CurrentLanguageInfo.ShortName}{Locals.StringTableFileExtension}");
		var content = File.ReadAllText(path);
		if (string.IsNullOrEmpty(content))
			return;
		var stringTableData = JsonConvert.DeserializeObject<SerializedStringTable>(content);
		_entries.Clear();
		_entries.AddRange(stringTableData.Content);
		LoadMainStringTable();
		while (_identifiers.Count > _entries.Count)
		{
			_entries.Add("TO DEFINE");
		}
	}

	private void LoadMainStringTable()
	{
		var path = MainStringTablePath;
		var content = File.ReadAllText(path);
		if (string.IsNullOrEmpty(content))
		{
			_mainStringTable = new SerializedMainStringTable();
			return;
		}
		_mainStringTable = JsonConvert.DeserializeObject<SerializedMainStringTable>(content);
		_identifiers.Clear();
		_identifiers.AddRange(_mainStringTable.Identifier);
		_mainStringTableAssigned = true;
	}
	
	protected override void Draw()
	{
		ImGui.Text("Localization Manager");
		ImGui.Spacing();

		if(IsDirty) 
			ImGui.TextColored(Vector4Colors.Red, "THERE ARE UNSAVED CHANGES!");
		ImGui.Separator();
		if (ImGui.Button("Close"))
		{
			Close();
		}

		if (CanSave && ImGui.Button("Save"))
		{
			SaveTable();
		}
		ImGui.Separator();
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();

		if (!_mainStringTableAssigned)
		{
			ShowTableCreation();
			return;
		}
		ImGui.Separator();
		if(LocalizationsExist && ImGui.Button(!_creatingLocal ? "Create new local" : "Cancel local creation"))
			_creatingLocal = !_creatingLocal;
		if (!LocalizationsExist || _creatingLocal)
		{
			ImGui.Spacing();
			ImGui.TextColored(Vector4Colors.Green, "Local Creation");
			ImGui.Text(_creatingLocal ? "Enter the proper values for the local." : "No Localizations found.");
			ImGui.Spacing();
			
			CreateLocalization();
			if (!LocalizationsExist)
				return;
			ImGui.Spacing();
			ImGui.Spacing();
		}
		ImGui.Separator();
		
		ImGui.TextColored(Vector4Colors.Green, "Local Data");
		
		var index = _currentLanguageIndex;
		if (ImGui.Combo("Localizations", ref index, _availableLocalizations, _localizationsPath.Count) && _currentLanguageIndex != index)
		{
			_currentLanguageIndex = index;
			LoadLocalization();
			//TODO: Load other localization
		}

		if (ImGui.Button("Add Item"))
		{
			_identifiers.Add(_identifiers.Count.ToString());
			_entries.Add("none");
			IsDirty = true;
		}

		ImGui.Spacing();
		ImGui.Spacing();
		if (_entries.Count == 0)
			ImGui.Text("No localization entry exists!");
		
		else if (ImGui.BeginTable("local", 2, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Identifier");
			ImGui.TableSetupColumn("Value");
			ImGui.TableHeadersRow();
			for (var i = 0; i < _entries.Count; i++)
				DrawRow(i);
			ImGui.EndTable();
		}
	}

	private void SaveTable()
	{
		SerializedStringTable stringTable = new([.. _entries]);
		_mainStringTable = new([.. _identifiers]);
		
		File.WriteAllText(MainStringTablePath, JsonConvert.SerializeObject(_mainStringTable));
		File.WriteAllText(CurrentStringTablePath, JsonConvert.SerializeObject(stringTable));
		IsDirty = false;
	}

	private void DrawRow(int i)
	{
		ImGui.TableNextColumn();
		var identifier = _identifiers[i];
		if (ImGui.InputText($"Identifier | {i}", ref identifier, 128))
		{
			_identifiers[i] = identifier;
			IsDirty = true;
		}
		ImGui.TableNextColumn();
		var value = _entries[i];
		if (ImGui.InputText($"Value | {i}", ref value, (int)short.MaxValue, ImGuiInputTextFlags.CtrlEnterForNewLine))
		{
			_entries[i] = value;
			IsDirty = true;
		}
	}

	private string _localIdentifier = "";
	private string _localFormalName = "";
	
	private void CreateLocalization()
	{
		ImGui.InputText("Local identification", ref _localIdentifier, 128);
		ImGui.InputText("Local formal name", ref _localFormalName, 128);
		var filePath = Path.Combine(LocalizationFolderPath, $"{_localIdentifier}{Locals.LocalInfoExtension}");
		var stringTablePath = Path.Combine(LocalizationFolderPath, $"{_localIdentifier}{Locals.StringTableFileExtension}");

		var canCreate = PathUtils.IsValidPath(filePath) && PathUtils.IsValidPath(stringTablePath) && !string.IsNullOrWhiteSpace(_localFormalName) && !string.IsNullOrWhiteSpace(_localIdentifier);
		if(!canCreate)
			ImGui.TextColored(Vector4Colors.Red, "Please enter valid values");
		if (ImGui.Button("Create Localization Table") && canCreate)
		{
			filePath = filePath.Trim();
			stringTablePath = stringTablePath.Trim();
			using var f = File.Create(filePath);
			f.Dispose();
			using var f2 = File.Create(stringTablePath);
			f2.Dispose();
			File.WriteAllText(filePath,
				JsonConvert.SerializeObject(new SerializedLanguageInfo(_localIdentifier, _localFormalName, Path.GetFileName(stringTablePath))));
			File.WriteAllText(stringTablePath, JsonConvert.SerializeObject(new SerializedStringTable(Array.Empty<string>())));
			FindLocalizations();
			LoadLocalization();
			_localIdentifier = "";
			_localFormalName = "";
		}
	}

	private void ShowTableCreation()
	{
		if (ImGui.Button("Create Localization Table"))
		{
			if(!Directory.Exists(LocalizationFolderPath))
				Directory.CreateDirectory(LocalizationFolderPath);
			using var f = File.Create(MainStringTablePath);
			_mainStringTable = new SerializedMainStringTable();
			_mainStringTableAssigned = true;
		}
	}
}