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

        protected bool InputFile(string label, ref string result)
        {
            bool modified = false;
            if (ImGui.Button("Search##"+label))
            {
                result = FileSearching.OpenFileSearch();
                modified = true;
            }
            ImGui.SameLine();
            ImGui.InputText(label, ref result, 1024);
            return modified;
        }

        protected bool InputDirectory(string label, ref string result)
        {
            bool modified = false;
            if (ImGui.Button("Search##"+label))
            {
                result = FileSearching.OpenFolderSearch();
                modified = true;
            }
            ImGui.SameLine();
            ImGui.InputText(label, ref result, 1024);
            return modified;
        }
    }
}
