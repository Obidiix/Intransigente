using Assimp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Content
{
    internal class Chest
    {
        public static AssimpContext AssimpContext { get; private set; }
        internal static void Initialize()
        {
            AssimpContext = new();
        }
    }
}
