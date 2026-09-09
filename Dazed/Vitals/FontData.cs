using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals
{
    public readonly struct FontData
    {
        [JsonProperty("p")]
        private readonly string _texturePath;
        [JsonProperty("w")]
        private readonly int _charWidth;
        [JsonProperty("h")]
        private readonly int _charHeight;
        [JsonProperty("c")]
        private readonly char[] _charOrder;

        [JsonConstructor]
        public FontData(string texPath, int charWidth, int charHeight, char[] charOrder)
        {
            _texturePath = texPath;
            _charWidth = charWidth;
            _charHeight = charHeight;
            _charOrder = charOrder;
        }

        [JsonIgnore]
        public string TextureRelativePath => _texturePath;
        [JsonIgnore]
        public int CharWidth => _charWidth;
        [JsonIgnore]
        public int CharHeight => _charHeight;
        [JsonIgnore]
        public char[] CharOrder => _charOrder;
    }
}
