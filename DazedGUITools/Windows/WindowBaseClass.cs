using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows
{
    public abstract class WindowBaseClass
    {
        internal readonly string _uniqueTitle;
        public readonly string Title;
        public readonly Guid UniqueWindowID;

        public bool MarkedForDisposal => _markedDisposing;
        private bool _markedDisposing;

        public WindowBaseClass(string title)
        {
            Title = title;
            UniqueWindowID = Guid.NewGuid();
            _uniqueTitle = GetUniqueName(title);
            MainWindow.AddWindow(this);
        }

        private string GetUniqueName(string title, int triesCount = 0)
        {
            string currentTitle = title + (triesCount > 0 ? ": " + triesCount.ToString() : "");
            if (!MainWindow.NameIsOccupied(currentTitle))
                return currentTitle;
            return GetUniqueName(title, ++triesCount);
        }

        protected void Close() => _markedDisposing = true;

        internal void CallDraw()
        {
            if (_markedDisposing)
                return;
            if (ImGui.Begin(_uniqueTitle))
            {
                Draw();
                ImGui.End();
            }
        }
        protected abstract void Draw();
    }
}
