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
        /// <summary>
        /// Called after path is set.
        /// </summary>
        public static void Initialize()
        {
            new BinExplorationWindow();
        }
    }
}
