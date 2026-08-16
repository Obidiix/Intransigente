using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Content
{
    public readonly struct ContentIdentification(string id, string path)
    {
        public string ID => id;
        public string Path => path;
    }
}
