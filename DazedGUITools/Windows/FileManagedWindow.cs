using Dazed.Vitals;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows
{
    public abstract class FileManagedWindow : WindowBaseClass
    {
        protected FileManagedWindow(string title, bool exclusive) : base(title, exclusive)
        {
        }

        protected void PathValidMessage(string path, out bool pathIsCompatible)
        {
            pathIsCompatible = PathUtils.IsValidPath(path);
            if (!pathIsCompatible)
                ImGui.TextColored(Vector4Colors.Red, "Please enter a proper path!");
        }

        protected void InputFile(string label, ref string result)
        {
            if (ImGui.Button("Search"))
                result = FileSearching.OpenFileSearch();
            ImGui.SameLine();
            ImGui.InputText(label, ref result, 1024);
        }

        protected void InputDirectory(string label, ref string result)
        {
            if (ImGui.Button("Search"))
                result = FileSearching.OpenFolderSearch();
            ImGui.SameLine();
            ImGui.InputText(label, ref result, 1024);
        }
    }
}
