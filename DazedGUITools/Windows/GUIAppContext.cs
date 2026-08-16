using Dazed.Vitals.Content;
using DazedGUITools.Windows.BinExploration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DazedGUITools.Windows
{
    internal static class GUIAppContext
    {
        internal static string ContentTableFilePath => Path.Combine(MainWindow.ProjectFolder, Chest.ContentTableFileName);

        /// <summary>
        /// Called after path is set.
        /// </summary>
        public static void Initialize()
        {
            SerializedContentTable? cT = null;
            if (File.Exists(ContentTableFilePath))
            {
                string content = File.ReadAllText(ContentTableFilePath);
                cT = JsonConvert.DeserializeObject<SerializedContentTable>(content);
            }
            if(cT == null)
                cT = new([], []);

            Chest.LoadContentTable(cT);
            new BinExplorationWindow();
        }
    }
}
