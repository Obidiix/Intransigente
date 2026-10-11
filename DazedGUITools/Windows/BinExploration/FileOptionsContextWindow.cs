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
            if (ImGui.Button("Close"))
                Close();
        }
    }
}
