using OpenTK.Graphics.OpenGL;
using Dazed.Vitals.Content;
using StbImageSharp;

namespace Dazed.Vitals.Rendering
{
    public class Texture2D : Texture
    {
        public ImageResult TextureImage
        {
            get => _mImage;
            set
            {
                if (_isReadOnly)
                {
                    if(!DebugState.DebugLow)
                        return;
                    Scriptorium.Warn("Image is readonly and can not be modified");
                    return;
                }
                _mImage = value;
                PushData();
            }
        }
        public float Width => _width;
        private float _width;
        public float Height => _height;
        private float _height;
        private byte[] _pixelsBytes;

        private ImageResult _mImage;
        private readonly bool _isReadOnly, _isTransparent;

        internal Texture2D(string path, bool isTransparent = false, bool isReadOnly = true)
            : base(TextureTarget.Texture2D)
        {
            _isTransparent = isTransparent;
            using (var stream = File.OpenRead(path))
            {
                _mImage = ImageResult.FromStream(stream, _isTransparent ? ColorComponents.RedGreenBlueAlpha : ColorComponents.RedGreenBlue);
            }
            _isReadOnly = isReadOnly;
            PushData();
        }
        private void UpdateByteArray()
        {
            _pixelsBytes = TextureImage.Data;
        }

        private void PushData()
        {
            _height = _mImage.Height;
            _width = _mImage.Width;
            UpdateByteArray();
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, _isTransparent ? PixelInternalFormat.Rgba2 : PixelInternalFormat.R3G3B2, TextureImage.Width, TextureImage.Height, 0, _isTransparent ? PixelFormat.Rgba : PixelFormat.Rgb, PixelType.UnsignedByte, _pixelsBytes);
            GL.TextureParameter(_textureHandle, TextureParameterName.GenerateMipmap, 1/*true*/);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            if (_isReadOnly)
                _pixelsBytes = Array.Empty<byte>();
        }
        //public void LoadNewTexture(IdentifiedAsset asset, bool pathIsAbsolute = false)
        //{
        //    if (_isReadOnly)
        //    {
        //        Scriptorium.Error($"Image trying to change it texture to asset: <{asset.ToString()}> but failed due to it being read only");
        //        return;
        //    }
        //    else
        //    {
        //        _mImage.Dispose();
        //        _mImage = new MagickImage(asset.AbsolutePath);
        //    }

        //    PushData();
        //}
        internal override void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
