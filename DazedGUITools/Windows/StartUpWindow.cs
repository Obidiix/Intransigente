using Dazed.Vitals;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows
{
    internal class StartUpWindow : FileManagedWindow
    {
        public string ResultPath { get; private set; } = "";
        string _currentSetPath = "";

        public StartUpWindow() : base("Starting up...", true)
        {
        }

        protected override void Draw()
        {
            ImGui.Text("Please select a path to use for resources and shaders");
            PathValidMessage(_currentSetPath, out bool pathIsCompatible);
            InputDirectory("Path", ref _currentSetPath);
            if(ImGui.Button("Select") && pathIsCompatible)
            {
                ResultPath = _currentSetPath;
                Close();
            }
        }
    }
}
