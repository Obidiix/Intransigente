using Dazed.Vitals.Content;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal static class Artist
    {
        static int _fbo;
        static int _rbo;
        static int _FrameBufferTexture;

        static FullscreenRenderer FullscreenRender;
        static readonly Shader _ScreenShader;
        static readonly Shader _testShader;
        static readonly Mesh _testMesh;
        static readonly Texture2D _testTexture;

        public static int Width { get => _width; internal set { _width = value; RefreshRenderingInfo(); } }
        internal static int _width;
        public static int Height { get => _height; internal set { _height = value; RefreshRenderingInfo(); } }
        internal static int _height;

        public static float RenderingScale { get => _renderingScale; set { _renderingScale = value < 0.1f ? 0.1f : value; RefreshRenderingInfo(); } }
        private static float _renderingScale = 0.5f;
        
        public static Vector2i RenderSize { get => _renderSize; }
        private static Vector2i _renderSize = new(1, 1);

        public static float NormalizedHeight => _normalizedHeight;
        private static float _normalizedHeight;

        public static float NormalizedWidth => _normalizedWidth;
        private static float _normalizedWidth;

        public static float AspectRatio { get => _aspectRatio; }
        private static float _aspectRatio;

        public static event Action OnWidowResize;

        private static void RefreshRenderingInfo()
        {
            _renderSize = new((int)(_width * _renderingScale), (int)(_height * _renderingScale));
            _aspectRatio = RenderSize.X / (float)RenderSize.Y;
            _normalizedHeight = 1 / (float)_height;
            _normalizedWidth = 1 / (float)_width;
            SetUpFrameBuffer();
            OnWidowResize?.Invoke();
        }

        public static void SetResolution(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;
            _width = width;
            _height = height;
            RefreshRenderingInfo();
        }

        static Artist()
        {
            _ScreenShader = new("screenShader");
            _testShader = new("projectedTest.vert", "test.frag");
            _testMesh = new(Chest.GetRelativePath("playerModel"), _testShader);
            _testTexture = new(Chest.GetRelativePath("missingTexture"));
        }

        internal static void Initialize()
        {
            SetUpFrameBuffer();
            FullscreenRender = new(_ScreenShader);
        }

        private static void SetUpFrameBuffer()
        {
            if (_fbo == 0)
                _fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            if (_FrameBufferTexture == 0)
                _FrameBufferTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _FrameBufferTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, RenderSize.X, RenderSize.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, 0);
            GL.TextureParameter(_FrameBufferTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TextureParameter(_FrameBufferTexture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_FrameBufferTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TextureParameter(_FrameBufferTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _FrameBufferTexture, 0);

            if (_rbo == 0)
                _rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, RenderSize.X, RenderSize.Y);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _FrameBufferTexture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                string message = "Frame buffer failed with status: " + status.ToString();
                Scriptorium.Error(message);
                throw new Exception(message);
            }
        }

        internal static void UpdateFrame(FrameEventArgs args)
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

            GL.Viewport(0, 0, RenderSize.X, RenderSize.Y);
            RenderMeshed();
            GL.Viewport(0, 0, Width, Height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            RenderFramebuffer();
            GUIRendering();
        }

        private static void RenderFramebuffer()
        {
            FullscreenRender.Render(_FrameBufferTexture);
        }

        private static void RenderMeshed()
        {
            GL.ClearColor(Color.LightCyan);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            
            ThreeDimensionsRendering();

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        private static void ThreeDimensionsRendering()
        {
            foreach (MeshPart part in _testMesh.MeshParts)
            {
                Shader shader = part.PartShader;
                _testTexture.Use();
                shader.SetMatrix4NotReference("view", DazedInstance.debugViewMatrix);
                shader.SetMatrix4NotReference("projection", DazedInstance.debugProjectionMatrix);
                _testShader.SetMatrix4NotReference("model", part.LocalMeshMatrix);

                shader.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
                shader.SetVector3("lightColor", new Vector3(1f, 1f, 1f));
                shader.SetVector3("lightPos", new Vector3(1.5f, 0, 3));
                shader.SetVector3("viewPos", DazedInstance.debugCameraPosition);

                part.RenderPart();
            }
        }

        private static void GUIRendering()
        {

        }
    }
}
