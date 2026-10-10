using ImGuiNET;
using System;

namespace DazedGUITools.Windows.LocalizationTools
{
    internal class LocalizationItemSelection() : WindowBaseClass("Localization item selection", false)
    {
        public string? ChosenItemIdentifier { get; private set; }
        public bool Completed { get; private set; }

        private string _filter = string.Empty;

        protected override void Draw()
        {
            if (ImGui.Button("Close"))
                Close();
            if (ImGui.InputText("Filter", ref _filter, (uint)short.MaxValue))
                _filter = _filter.ToLower();
            if(ImGui.BeginTable("Items", 2))
            {
                ImGui.TableSetupColumn("Identifier");
                ImGui.TableSetupColumn("Current local value");
                ImGui.TableHeadersRow();
                bool drawn = false;
                for (var i = 0; i < LocalizationData.ItemsCount; i++)
                {
                    if (!LocalizationData.GetIdentifier(i).ToLower().Contains(_filter))
                        continue;
                    DrawRow(i);
                    drawn = true;
                }
                ImGui.EndTable();
                if (!drawn)
                    ImGui.Text("No item was found with the filter assigned");
            }
            if (Completed)
                Close();
        }

        private void DrawRow(int i)
        {
            ImGui.TableNextColumn();
            var identifier = LocalizationData.GetIdentifier(i);
            if (ImGui.Button($"{i + 1}: {identifier}"))
            {
                ChosenItemIdentifier = identifier;
                Completed = true;
            }
            ImGui.TableNextColumn();
            var value = LocalizationData.GetEntry(i);
            ImGui.Text(value);
        }
    }
}
