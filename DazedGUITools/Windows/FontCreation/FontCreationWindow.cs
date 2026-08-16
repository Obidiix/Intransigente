using Dazed.Vitals;
using Dazed.Vitals.Rendering.TextRendering;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DazedGUITools.Windows.FontCreation
{
    [MainMenuWindow("Font creation")]
    public class FontCreationWindow : FileManagedWindow
    {
        private const string DefaultCharacterOrder = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789";

        string _fontName = string.Empty;
        string _currentSetTexturePath = string.Empty;
        string _fontChars = string.Empty;
        int _charWidth, _charHeight;

        bool? _result;

        public FontCreationWindow() : base("Font Creation", true)
        {
        }

        protected override void Draw()
        {
            ImGui.Text("Font creation window");
            ImGui.InputText("Name", ref _fontName, 1024);

            ImGui.Spacing();
            PathValidMessage(_currentSetTexturePath, out bool pathIsCompatible);
            InputFile("Texture path", ref _currentSetTexturePath);

            ImGui.Spacing();
            ImGui.Text("Insert all characters present in the font atlas");
            if (ImGui.SmallButton("Default"))
                _fontChars = DefaultCharacterOrder;
            ImGui.SameLine();
            ImGui.InputText("Chars", ref _fontChars, 1024);
            ImGui.Spacing();

            ImGui.InputInt("Width", ref _charWidth);
            ImGui.InputInt("Height", ref _charHeight);

            bool compatibleValues = pathIsCompatible && _fontName.Length > 0 && _fontChars.Length > 0 && _charHeight > 0 && _charWidth > 0;

            if (_result.HasValue)
                if (_result.Value)
                    ImGui.TextColored(Vector4Colors.Green, "Result: Successful");
                else
                    ImGui.TextColored(Vector4Colors.Red, "Result: Failed");

            if (!compatibleValues)
                ImGui.TextColored(Vector4Colors.Red, "One of the values above has an incompatible value.");

            if(ImGui.Button("Create") && compatibleValues)
                _result = CreateFontDataFile();
            if (ImGui.Button("Close"))
                Close();
        }

        private bool CreateFontDataFile()
        {
            try
            {
                string relativePath = PathUtils.ToRelativePath(_currentSetTexturePath);
                string baseDirectory = Path.Combine(MainWindow.ProjectFolder, "resources", Directory.GetParent(relativePath).Name);
                string fontDataPath = Path.Combine(baseDirectory, _fontName + ".fd");
                FontData data = new(relativePath, _charWidth, _charHeight, _fontChars.ToCharArray());
                string content = JsonConvert.SerializeObject(data);
                if (!File.Exists(fontDataPath))
                {
                    using var f = File.Create(fontDataPath);
                    f.Dispose();
                }
                File.WriteAllText(fontDataPath, content);
                return true;
            }
            catch(Exception ex)
            {
                Scriptorium.LogException(ex);
                return false;
            }
        }
    }
}
