using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Dazed.Vitals
{
    public class Font
    {
        public readonly Texture2D FontTexture;
        private readonly Dictionary<char, Vector2> _charTexPos = new();
        public readonly float CharacterAspectRatio;

        private int CharAmountX => _nx;
        private int CharAmountY => _ny;
        private readonly int _nx, _ny;

        public Vector2 TextureCharScale { get; }

        private static readonly Dictionary<string, Font?> FontInstances = new();

        public static Font GetFontInstance(IdentifiedAsset asset)
        {
            if (!FontInstances.TryGetValue(asset.RelativePath, out var font))
            {
                font = new Font(asset.RelativePath);
                FontInstances.Add(asset.RelativePath, font);
            }
            return font ?? throw new Exception("Font failed to be created with asset: " + asset.ToString());
        }

        private Font(string fontDataPath)
        {
            //No testing for file existence, as to ensure the game throwing an exception in case of the font not existing
            string content = File.ReadAllText(fontDataPath);
            FontData data = JsonConvert.DeserializeObject<FontData>(content);

            FontTexture = new Texture2D(data.TextureRelativePath, true);
            var charWidth = data.CharWidth;
            var charHeight = data.CharHeight;

            CharacterAspectRatio = charWidth / (float)charHeight;

            //Number of characters in X
            _nx = (int)(FontTexture.Width / (float)data.CharWidth);
            //Number of characters in Y
            _ny = (int)(FontTexture.Height / (float)data.CharHeight);
         
            int currentX = 0, currentY = 0;
            foreach (var t in data.CharOrder)
            {
                if (currentX == _nx)
                {
                    currentX = 0;
                    currentY++;
                }
                //TEMPORARY PLACED TO ALLOW CAPS ONLY FONT
                char c = char.ToLower(t);
                _charTexPos.Add(c, new(currentX, currentY));
                currentX++;
            }
            TextureCharScale = new Vector2(1 / (float)CharAmountX, 1 / (float)CharAmountY);
        }

        public Font(int charWidth)
        {
            var charWidth1 = charWidth;
        }

        public Vector2 GetCharPosInAtlas(char c, bool normalized = true)
        {
            //TEMPORARY PLACED TO ALLOW CAPS ONLY FONT
            c = char.ToLower(c);
            if (!_charTexPos.TryGetValue(c, out Vector2 pos))
            {
                Scriptorium.Warn("Font is missing character: " + c);
                return Vector2.Zero;
            }
            if (normalized)
                pos = new Vector2(pos.X / _nx, pos.Y / _ny);
            return pos;
        }
    }
}
