using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows
{
    internal class ExampleWindow : WindowBaseClass
    {
        public ExampleWindow() : base("Example Window", false)
        {
        }

        protected override void Draw()
        {
            ImGui.Text("This is an example!");
            if (ImGui.Button("Close"))
                Close();
        }
    }
}
