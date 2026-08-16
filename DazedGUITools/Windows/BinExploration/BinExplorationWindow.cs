using Dazed.Vitals;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DazedGUITools.Windows.BinExploration
{
    internal class BinExplorationWindow : WindowBaseClass
    {
        private string _currentSubFolder = "";
        private string CurrentPath => _currentSubFolder == "" ? MainWindow.ProjectFolder : Path.Combine(MainWindow.ProjectFolder, _currentSubFolder);

        private bool _contextChanged = true;

        private DisplayedBinElement[] _displayedElements = []; 

        public BinExplorationWindow() : base("Bin Explorer", true)
        {
        }

        protected override void Draw()
        {
            if (_contextChanged)
                ReloadContext();
            if (_currentSubFolder != string.Empty && ImGui.Button(".."))
            {
                _contextChanged = true;
                _currentSubFolder = DirectoryDown(_currentSubFolder);
                return;
            }
            for(int i = 0; i < _displayedElements.Length; i++)
            {
                if (ImGui.Button(_displayedElements[i].RelativePath))
                    SelectElement(_displayedElements[i]);
            }
        }

        private static string DirectoryDown(string currentSubFolder)
        {
            if (!currentSubFolder.Contains('/'))
                return string.Empty;
            string result = "";
            int count = 0;
            for(int i = currentSubFolder.Length - 1; i >= 0; i++)
            {
                if (currentSubFolder[i] == '/')
                    break;
                count++;
            }
            for(int i = 0; i < currentSubFolder.Length - count; i++)
                result += currentSubFolder[i];
            return result;
        }

        private void SelectElement(DisplayedBinElement displayedBinElement)
        {
            if (displayedBinElement.IsFolder)
                SelectAsFolder(displayedBinElement);
            else
                SelectAsFile(displayedBinElement);
        }

        private FileOptionsContextWindow cWindow;
        private void SelectAsFile(DisplayedBinElement displayedBinElement)
        {
            if (cWindow != null)
                if (!cWindow.MarkedForDisposal)
                    return;
            cWindow = new(displayedBinElement.RelativePath, _currentSubFolder);
        }

        private void SelectAsFolder(DisplayedBinElement displayedBinElement)
        {
            _contextChanged = true;
            _currentSubFolder = _currentSubFolder == string.Empty ? displayedBinElement.RelativePath : Path.Combine(_currentSubFolder, displayedBinElement.RelativePath);
        }

        private void ReloadContext()
        {
            DisplayedBinElement[] fileElements = FindAllFiles();
            DisplayedBinElement[] directoryElements = FindAllDirectories();

            _displayedElements = new DisplayedBinElement[fileElements.Length + directoryElements.Length];

            for (int d = 0; d < directoryElements.Length; d++)
                _displayedElements[d] = directoryElements[d];
            for (int f = 0; f < fileElements.Length; f++)
                _displayedElements[directoryElements.Length + f] = fileElements[f];
        }

        private DisplayedBinElement[] FindAllDirectories()
        {
            string[] directories = Directory.GetDirectories(CurrentPath);
            DisplayedBinElement[] elements = new DisplayedBinElement[directories.Length];
            for(int i = 0; i < directories.Length; i++)
                elements[i] = new(true, PathUtils.GetToLastDash(directories[i]));
            return elements;
        }

        private DisplayedBinElement[] FindAllFiles()
        {
            string[] files = Directory.GetFiles(CurrentPath);
            List<DisplayedBinElement> elements = new();
            for(int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll") || files[i].EndsWith(".exe") || files[i].EndsWith(".pdb"))
                    continue;
                elements.Add(new(false, PathUtils.GetToLastDash(files[i])));
            }
            return elements.ToArray();
        }
    }
}
