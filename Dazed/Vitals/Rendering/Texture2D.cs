using OpenTK.Graphics.OpenGL;
using Dazed.Vitals.Content;
using StbImageSharp;

namespace Dazed.Vitals.Rendering
{
    public class Texture2D : Texture
    {
        public ImageResult TextureImage
        {
            get => _image;
            set
            {
                if (_isReadOnly)
                {
                    if(!DebugState.DebugLow)
                        return;
                    Scriptorium.Warn("Image is readonly and can not be modified");
                    return;
                }
                _image = value;
                PushData();
            }
        }
        public float Width => _width;
        private float _width;
        public float Height => _height;
        private float _height;

        public ImageDataType ImageDataType => _dataType;
        private ImageDataType _dataType;
        private byte[] _pixelsBytes = null!;

        private ImageResult _image;
        private readonly bool _isReadOnly;

        private static readonly Dictionary<TextureIdentification, Texture2D> InstantiatedTextures = new();
        public static Texture2D GetTextureInstance(TextureIdentification identification)
        {
            if(InstantiatedTextures.TryGetValue(identification, out var texture))
                return texture;
            texture = new(identification.Path, identification.ImageDataType, identification.IsReadOnly);
            InstantiatedTextures.Add(identification, texture);
            return texture;
        }
        public static Texture2D GetTextureInstance(string path) => GetTextureInstance(new TextureIdentification(path, ImageDataType.RGB));

        internal Texture2D(string path, ImageDataType dataType, bool isReadOnly)
            : base(TextureTarget.Texture2D)
        {
            _dataType = dataType;
            using (var stream = File.OpenRead(path))
            {
                _image = ImageResult.FromStream(stream, GetColorComponent());
            }

            _isReadOnly = isReadOnly;
            PushData();
        }

        private ColorComponents GetColorComponent()
        {
            return _dataType switch
            {
                ImageDataType.RGB => ColorComponents.RedGreenBlue,
                ImageDataType.RGBA => ColorComponents.RedGreenBlueAlpha,
                ImageDataType.Grey => ColorComponents.Grey,
                _ => throw new NotImplementedException()
            };
        }

        private void UpdateByteArray()
        {
            if (_isReadOnly)
                return;
            _pixelsBytes = TextureImage.Data;
        }

        private void PushData()
        {
            _height = _image.Height;
            _width = _image.Width;
            UpdateByteArray();
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, TranslatePixelInternalFormat(), TextureImage.Width, TextureImage.Height, 0, TranslatePixelFormat(), PixelType.UnsignedByte, TextureImage.Data);
            GL.TextureParameter(_textureHandle, TextureParameterName.GenerateMipmap, 1/*true*/);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            if (_isReadOnly)
                _pixelsBytes = Array.Empty<byte>();
        }
        public PixelInternalFormat TranslatePixelInternalFormat()
        {
            return _dataType switch
            {
                ImageDataType.RGB => PixelInternalFormat.R3G3B2,
                ImageDataType.RGBA => PixelInternalFormat.Rgba2,
                ImageDataType.Grey => PixelInternalFormat.Intensity4,
            };
        }
        public PixelFormat TranslatePixelFormat()
        {
            return _dataType switch
            {
                ImageDataType.RGB => PixelFormat.Rgb,
                ImageDataType.RGBA => PixelFormat.Rgba,
                ImageDataType.Grey => PixelFormat.Rgb,
            };
        }
        internal override void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
