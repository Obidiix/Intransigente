using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public readonly struct TextureIdentification(string path, ImageDataType imageDataType, bool alphaIsTransparency = true, bool isReadOnly = true)
    {
        public readonly string Path => path;
        public readonly ImageDataType ImageDataType => imageDataType;
        public readonly bool AlphaIsTransparency => alphaIsTransparency;
        public readonly bool IsReadOnly => isReadOnly;
    }
}
