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
            }
        }

        byte[] pixelsBytes;

        MagickImage _dImage;
        readonly MagickImage _roImage;
        readonly bool _isReadOnly;

        public Texture2D(string path, bool pathIsAbsolute = false, bool isReadOnly = true) : base(TextureTarget.Texture2D)
        {
            _isReadOnly = isReadOnly;

            if (!pathIsAbsolute)
                path = Path.Combine(PathUtils.ResourcesDirectory, path);

            if (_isReadOnly)
                _roImage = new MagickImage(path);
            else
                _dImage = new MagickImage(path);
            TextureImage.Format = MagickFormat.Rgb;
            PushChanges();
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, (int)TextureImage.BaseWidth, (int)TextureImage.BaseHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixelsBytes);
            GL.TextureParameter(_textureHandle, TextureParameterName.GenerateMipmap, 1/*true*/);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_textureHandle, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void PushChanges()
        {
            if (pixelsBytes != null)
                if (_isReadOnly)
                    return;
            pixelsBytes = TextureImage.ToByteArray();
        }
        internal override void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
