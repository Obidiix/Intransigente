using ImageMagick;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class Texture2D : Texture
    {
        public MagickImage TextureImage
        {
            get => _isReadOnly ? _roImage : _dImage;
            set
            {
                if (_isReadOnly)
                {
                    Scriptorium.Warn("Image is readonly and can not be modified");
                    return;
                }
                _dImage = value;
                PushData();
            }
        }
        public float Width => TextureImage.Width;
        public float Height => TextureImage.Height;

        byte[] pixelsBytes;

        MagickImage _dImage;
        readonly MagickImage _roImage;
        readonly bool _isReadOnly, _isTransparent;

        public Texture2D(string path, bool isTransparent = false, bool pathIsAbsolute = false, bool isReadOnly = true) : base(TextureTarget.Texture2D)
        {
            _isReadOnly = isReadOnly;
            _isTransparent = isTransparent;
            if (!pathIsAbsolute)
                path = PathUtils.BaseDirectoryCombine(path);

            if (_isReadOnly)
                _roImage = new MagickImage(path);
            else
                _dImage = new MagickImage(path);

            PushData();
        }
        private void UpdateByteArray()
        {
            if (pixelsBytes != null)
                if (_isReadOnly)
                    return;
            pixelsBytes = TextureImage.ToByteArray();
        }
        public void PushData()
        {
            TextureImage.Format = _isTransparent ? MagickFormat.Rgba : MagickFormat.Rgb;
            UpdateByteArray();
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, _isTransparent ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb, (int)TextureImage.BaseWidth, (int)TextureImage.BaseHeight, 0, _isTransparent ? PixelFormat.Rgba : PixelFormat.Rgb, PixelType.UnsignedByte, pixelsBytes);
            GL.TextureParameter(_textureHandle, TextureParameterName.GenerateMipmap, 1/*true*/);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void LoadNewTexture(string path, bool pathIsAbsolute = false)
        {
            if (!pathIsAbsolute)
                path = PathUtils.BaseDirectoryCombine(path);

            if (_isReadOnly)
            {
                Scriptorium.Error($"Image trying to change it texture to path: <{path}> but failed due to it being read only");
                return;
            }
            else
            {
                _dImage.Dispose();
                _dImage = new MagickImage(path);
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
