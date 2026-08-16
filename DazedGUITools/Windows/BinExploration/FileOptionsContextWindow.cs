using Dazed.Vitals.Content;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DazedGUITools.Windows.BinExploration
{
    internal class FileOptionsContextWindow : WindowBaseClass
    {
        string _exposedID = null;
        readonly string _currentRelativePath, _fileName;
        string RelativeFilePath => Path.Combine(_currentRelativePath, _fileName);
        public FileOptionsContextWindow(string fileName, string currentRelativePath) : base("File Context", true)
        {
            _currentRelativePath = currentRelativePath;
            _fileName = fileName;
        }

        protected override void Draw()
        {
            ImGui.Text($"For file: {RelativeFilePath}");
            ShowExposingOptions();
            if (ImGui.Button("Close"))
                Close();
        }

        private void ShowExposingOptions()
        {
            if(Chest.ValueExists(RelativeFilePath))
            {
                if(_exposedID == null)
                    _exposedID = Chest.GetId(Path.Combine(_currentRelativePath, _fileName));
                ImGui.Text("Is exposed with id value: " + _exposedID);
                ImGui.SameLine();
                if(ImGui.Button("Un-expose file"))
                {
                    Chest.RemoveId(_exposedID, out bool b);
                    if (!b)
                        ThisShouldNotHappen();
                    _exposedID = string.Empty;
                }
                
                return;
            }

            if (_exposedID == null)
                _exposedID = string.Empty;

            bool IdOccupied = Chest.IdExists(_exposedID);
            if (IdOccupied)
                ImGui.TextColored(new(1, 0, 0, 1), "This ID is already occupied!");
            if(ImGui.Button("Expose with ID") && !string.IsNullOrEmpty(_exposedID) && !IdOccupied)
            {
                Chest.AddId(new(_exposedID, RelativeFilePath), out bool b);
                if (!b)
                    ThisShouldNotHappen();
                return;
            }
            ImGui.SameLine();
            ImGui.InputText("ID", ref _exposedID, 1024);
        }

        private static void ThisShouldNotHappen()
        {
            throw new Exception("This should not happen, but it happened");
        }
    }
}
