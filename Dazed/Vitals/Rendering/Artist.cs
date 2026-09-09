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

        public static Texture2D MissingTexture => _missingTexture;
        static readonly Texture2D _missingTexture;

        public static int Width { get => _width; internal set { _width = value; RefreshRenderingInfo(); } }
        internal static int _width;
        public static int Height { get => _height; internal set { _height = value; RefreshRenderingInfo(); } }
        internal static int _height;

        public static float RenderingScale { get => _renderingScale; set { _renderingScale = value < 0.1f ? 0.1f : value; RefreshRenderingInfo(); } }
        private static float _renderingScale = 1f;
        
        public static Vector2i RenderSize { get => _renderSize; }
        private static Vector2i _renderSize = new(1, 1);

        public static float NormalizedHeight => _normalizedHeight;
        private static float _normalizedHeight;

        public static float NormalizedWidth => _normalizedWidth;
        private static float _normalizedWidth;

        public static float AspectRatio { get => _aspectRatio; }
        private static float _aspectRatio;

        public static float RenderAspectRatio => _renderAspectRatio;
        private static float _renderAspectRatio;

        public static Vector2 WinPos => _winPos;
        private static Vector2 _winPos;

        public static event Action OnWidowResize;

        const int RenderElementsLimit = 4096;
        private static readonly Renderable?[] _allRender = new Renderable[RenderElementsLimit];
        private static readonly RenderingPass[] _renderPasses = new RenderingPass[RenderElementsLimit];

        private static readonly List<Camera> _cameras = [];
        private static readonly CameraPriorityComparer Comparer = new();

        private static List<Renderable> _geometryFirst = new(), _geometrySecond = new(), _gui = new();
        private static bool _modifiedRendered = true;

        private static int _currentBindedPosition;

        private static readonly Texture2D _screenShaderData;

        private static void RefreshRenderingInfo()
        {
            _renderSize = new((int)(_width * _renderingScale), (int)(_height * _renderingScale));
            _renderAspectRatio = RenderSize.X / (float)RenderSize.Y;
            _aspectRatio = _height / (float)_width;
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

        internal static void ReloadWindowPosition(Vector2 pos) => _winPos = pos;

        static Artist()
        {
            _ScreenShader = Shader.GetShaderInstance("screenShader");
            
            _missingTexture = new(Chest.GetRelativePath("missingTexture"));
            _screenShaderData = new(Chest.GetRelativePath("screenShaderDataImage"));
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

        internal static void TriggerAllShadersReload()
        {
            Scriptorium.Warn("Reloading all shader...");
            Shader.ReloadShaders();
        }

        internal static int AssignRendered(Renderable rendered)
        {
            if (_currentBindedPosition >= RenderElementsLimit)
                throw new IndexOutOfRangeException("Drawn mesh limit exceeded, either increase arbitrary limit or check for duplicated meshes");
            _renderPasses[_currentBindedPosition] = rendered.RenderPass;
            _allRender[_currentBindedPosition++] = rendered;

            _modifiedRendered = true;
            return _currentBindedPosition;
        }

        internal static void DisposeRendered(int bindPosition)
        {
            _modifiedRendered = true;
            _allRender[bindPosition] = null;
        }

        internal static void AddCamera(Camera camera)
        {
            _cameras.Add(camera);
            ReorderCameras();
        }

        private static void ReorderCameras()
        {
            if (_cameras.Count <= 1)
                return;
            _cameras.Sort(Comparer);
        }

        internal static void RemoveCamera(Camera camera)
        {
            _cameras.Remove(camera);
            ReorderCameras();
        }

        internal static void DrawFrame(FrameEventArgs args)
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

            GL.Viewport(0, 0, RenderSize.X, RenderSize.Y);
            RenderMeshed();
            GL.Viewport(0, 0, Width, Height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            RenderFramebuffer();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GUIRendering();
        }

        private static void RenderFramebuffer()
        {
            FullscreenRender.Render(_FrameBufferTexture, _screenShaderData);
        }

        private static void RenderMeshed()
        {
            GL.ClearColor(Color.LightCyan);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            ArrangePasses();

            for(int c = 0; c < _cameras.Count; c++)
            {
                RenderingData data = new(_cameras[c].Projection, _cameras[c].View, (Vector3)_cameras[c].Transform.Position);
                if (_cameras[c].RenderingPasses.HasFlag(RenderingPass.geometryFirst))
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    RenderPass(RenderingPass.geometryFirst, ref data);
                }
                if (_cameras[c].RenderingPasses.HasFlag(RenderingPass.geometrySecond))
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    RenderPass(RenderingPass.geometrySecond, ref data);
                }
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        private static void ArrangePasses()
        {
            if (!_modifiedRendered)
                return;
            _modifiedRendered = false;
            _geometryFirst.Clear();
            _geometrySecond.Clear();
            _gui.Clear();
            for(int i = 0; i < _currentBindedPosition; i++)
            {
                if (_allRender == null)
                    continue;
                switch (_renderPasses[i])
                {
                    case RenderingPass.geometryFirst:
                        _geometryFirst.Add(_allRender[i]);
                        break;
                    case RenderingPass.geometrySecond:
                        _geometrySecond.Add(_allRender[i]);
                        break;
                    case RenderingPass.userInterface:
                        _gui.Add(_allRender[i]);
                        break;
                }
            }
        }

        private static void RenderPass(RenderingPass pass, ref RenderingData data)
        {
            List<Renderable> collection = null;
            switch (pass)
            {
                case RenderingPass.geometryFirst:
                    collection = _geometryFirst;
                    break;
                case RenderingPass.geometrySecond:
                    collection = _geometrySecond;
                    break;
                case RenderingPass.userInterface:
                    collection = _gui;
                    break;
            }
            foreach(Renderable renderable in collection)
            {
                if (!renderable.AllowRendering)
                    continue;
                renderable.Render(data);
            }
        }

        private static void RenderPass(RenderingPass pass)
        {
            List<Renderable> collection = null;
            switch (pass)
            {
                case RenderingPass.geometryFirst:
                    collection = _geometryFirst;
                    break;
                case RenderingPass.geometrySecond:
                    collection = _geometrySecond;
                    break;
                case RenderingPass.userInterface:
                    collection = _gui;
                    break;
            }
            foreach (Renderable renderable in collection)
            {
                if (!renderable.AllowRendering)
                    continue;
                renderable.Render(default);
            }
        }

        private static void GUIRendering()
        {
            RenderPass(RenderingPass.userInterface);
        }
    }
}
