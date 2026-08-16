using System;
using System.Collections.Generic;
using System.Text;

namespace DazedGUITools.Windows
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class MainMenuWindowAttribute : Attribute
    {
        public readonly string Name;
        public MainMenuWindowAttribute(string name) => Name = name;
    }
}
