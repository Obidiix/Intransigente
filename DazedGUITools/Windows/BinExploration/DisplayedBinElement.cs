using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows.BinExploration
{
    internal readonly struct DisplayedBinElement(bool isFolder, string relativePath)
    {
        public bool IsFolder => isFolder;
        public string RelativePath => relativePath;
    }
}
