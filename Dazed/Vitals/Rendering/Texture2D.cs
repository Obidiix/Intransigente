using ImageMagick;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using Dazed.Vitals.Content;

namespace Dazed.Vitals.Rendering
{
    public class Texture2D : Texture
    {
        public MagickImage TextureImage
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
        public float Width => TextureImage.Width;
        public float Height => TextureImage.Height;

        private byte[] _pixelsBytes;

        private MagickImage _mImage;
        private readonly bool _isReadOnly, _isTransparent;

        public Texture2D(IdentifiedAsset asset, bool isTransparent = false, bool pathIsAbsolute = false, bool isReadOnly = true) : base(TextureTarget.Texture2D)
        {
            _isTransparent = isTransparent;
            _mImage = new MagickImage(asset.AbsolutePath);
            PushData();
            _isReadOnly = isReadOnly;
        }

        internal Texture2D(string path, bool isTransparent = false, bool isReadOnly = true, bool pathIsAbsolute = false)
            : base(TextureTarget.Texture2D)
        {
            _isTransparent = isTransparent;
            _mImage = new MagickImage(path);
            PushData();
            _isReadOnly = isReadOnly;
        }
        private void UpdateByteArray()
        {
            if (_isReadOnly)
                return;
            _pixelsBytes = TextureImage.ToByteArray();
        }

        private void PushData()
        {
            TextureImage.Format = _isTransparent ? MagickFormat.Rgba : MagickFormat.Rgb;
            UpdateByteArray();
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, _isTransparent ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb, (int)TextureImage.BaseWidth, (int)TextureImage.BaseHeight, 0, _isTransparent ? PixelFormat.Rgba : PixelFormat.Rgb, PixelType.UnsignedByte, _pixelsBytes);
            GL.TextureParameter(_textureHandle, TextureParameterName.GenerateMipmap, 1/*true*/);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void LoadNewTexture(IdentifiedAsset asset, bool pathIsAbsolute = false)
        {
            if (_isReadOnly)
            {
                Scriptorium.Error($"Image trying to change it texture to asset: <{asset.ToString()}> but failed due to it being read only");
                return;
            }
            else
            {
                _mImage.Dispose();
                _mImage = new MagickImage(asset.AbsolutePath);
            }

            PushData();
        }
        internal override void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
