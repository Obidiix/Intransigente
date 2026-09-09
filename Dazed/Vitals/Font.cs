using Dazed.Vitals.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Dazed.Vitals
{
    public class Font
    {
        public readonly Texture2D _fontTexture;
        public readonly int _charHeight, _charWidth;
        private readonly Dictionary<char, Vector2> _charTexPos;
        public readonly float _characterAspectRatio;

        public int CharAmountX => Nx;
        public int CharAmountY => Ny;

        public Vector2 TextureCharScale => _texCharScale;
        private readonly Vector2 _texCharScale;

        private readonly int Nx, Ny;

        private readonly static Dictionary<string, Font> _fontInstances = new();

        public static Font GetFontInstance(string fontDataPath)
        {
            if (!_fontInstances.TryGetValue(fontDataPath, out Font f))
            {
                f = new Font(fontDataPath);
                _fontInstances.Add(fontDataPath, f);
            }
            return f;
        }

        private Font(string fontDataPath)
        {
            //No testing for file existence, as to ensure the game throwing an exception in case of the font not existing
            string content = File.ReadAllText(fontDataPath);
            FontData data = JsonConvert.DeserializeObject<FontData>(content);

            _fontTexture = new(data.TextureRelativePath, true);
            _charWidth = data.CharWidth;
            _charHeight = data.CharHeight;

            _characterAspectRatio = _charWidth / (float)_charHeight;

            _charTexPos = new();
            //Number of characters in X
            Nx = (int)(_fontTexture.Width / (float)data.CharWidth);
            //Number of characters in Y
            Ny = (int)(_fontTexture.Height / (float)data.CharHeight);
         
            int currentX = 0, currentY = 0;
            for(int i = 0; i < data.CharOrder.Length; i++)
            {
                if (currentX == Nx)
                {
                    currentX = 0;
                    currentY++;
                }
                //TEMPORARY PLACED TO ALLOW CAPS ONLY FONT
                char c = char.ToLower(data.CharOrder[i]);
                _charTexPos.Add(c, new(currentX, currentY));
                currentX++;
            }
            _texCharScale = new Vector2(1 / (float)CharAmountX, 1 / (float)CharAmountY);
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
                pos = new(pos.X / Nx, pos.Y / Ny);
            return pos;
        }
    }
}
