using Dazed.Vitals;
using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools
{
    internal static class FileSearching
    {
        public static string OpenFolderSearch()
        {
            string result = "";
            System.Windows.Forms.FolderBrowserDialog dialog = new()
            {
                Multiselect = false,
                Description = "Select folder"
            };
            System.Windows.Forms.DialogResult dResult = dialog.ShowDialog();
            if (dResult == System.Windows.Forms.DialogResult.OK)
                result = dialog.SelectedPath;
            return result;
        }

        internal static string OpenFileSearch()
        {
            string result = "";
            System.Windows.Forms.OpenFileDialog dialog = new()
            {
                Multiselect = false,
                Title = "Select file"
            };
            System.Windows.Forms.DialogResult dResult = dialog.ShowDialog();
            if (dResult == System.Windows.Forms.DialogResult.OK)
                result = dialog.FileName;
            return result;
        }
    }
}
