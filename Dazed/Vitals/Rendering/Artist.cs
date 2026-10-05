using Dazed.Vitals.Content;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using Dazed.Vitals.Rendering.RenderDebug;
using OpenTK.Mathematics;

namespace Dazed.Vitals.Rendering
{
    internal static class Artist
    {
        static int _fbo;
        static int _rbo;
        static int _frameBufferTexture;

        private static FullscreenRenderer _fullscreenRender = null!;
        private static readonly Shader ScreenShader;

        public static Texture2D MissingTexture { get; }

        public static int Width { get => _width; internal set { _width = value; RefreshRenderingInfo(); } }
        private static int _width;
        public static int Height { get => _height; internal set { _height = value; RefreshRenderingInfo(); } }
        private static int _height;

        public static float RenderingScale { get => _renderingScale; set { _renderingScale = value < 0.1f ? 0.1f : value; RefreshRenderingInfo(); } }
        private static float _renderingScale = 1f;

        private static Vector2i RenderSize => _renderSize;
        private static Vector2i _renderSize = new(1, 1);

        public static float NormalizedHeight => _normalizedHeight;
        private static float _normalizedHeight;

        public static float NormalizedWidth => _normalizedWidth;
        private static float _normalizedWidth;

        public static float AspectRatio { get => _aspectRatio; }
        private static float _aspectRatio;

        public static float RenderAspectRatio => _renderAspectRatio;
        private static float _renderAspectRatio;

        public static Vector2i WinPos => _winPos;
        private static Vector2i _winPos;

        public static event Action OnWidowResize = null!;

        const int RenderElementsLimit = 4096;
        private static readonly Renderable?[] AllRender = new Renderable[RenderElementsLimit];
        private static readonly RenderingPass[] RenderPasses = new RenderingPass[RenderElementsLimit];

        private static ViewportCamera? _secondaryCamera;

        private static readonly List<TemporalRenderable> TemporalRenderables = new();
        
        private static readonly List<int> GeometryFirst = new();
        private static readonly List<int> GeometrySecond = new();
        private static readonly List<int> Gui = new();
        private static bool _modifiedRendered = true;

        private static int _currentBindedPosition;

        private static readonly Texture2D ScreenEffectTexture2D;

        public static void ReloadScreenInfo()
        {
            SetUpFrameBuffer();
            OnWidowResize?.Invoke();
        }
        private static void RefreshRenderingInfo()
        {
            _renderSize = new((int)(_width * _renderingScale), (int)(_height * _renderingScale));
            _renderAspectRatio = RenderSize.X / (float)RenderSize.Y;
            _aspectRatio = _height / (float)_width;
            _normalizedHeight = 1 / (float)_height;
            _normalizedWidth = 1 / (float)_width;

            ReloadScreenInfo();
        }

        public static void SetResolution(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;
            _width = width;
            _height = height;
            RefreshRenderingInfo();
        }

        internal static void ReloadWindowPosition(Vector2i pos) => _winPos = pos;

        static Artist()
        {
            ScreenShader = Shader.GetShaderInstance("screenShader");
            IdentifiedAsset missingTextureAsset = new("missingTexture");
            IdentifiedAsset screenEffectAsset = new("screenShaderDataImage");
            MissingTexture = new Texture2D(missingTextureAsset);
            ScreenEffectTexture2D = new Texture2D(screenEffectAsset);
        }

        internal static void Initialize()
        {
            SetUpFrameBuffer();
            _fullscreenRender = new FullscreenRenderer(ScreenShader);
        }

        private static void SetUpFrameBuffer()
        {
            if (_fbo == 0)
                _fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            if (_frameBufferTexture == 0)
                _frameBufferTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _frameBufferTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, RenderSize.X, RenderSize.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, 0);
            GL.TextureParameter(_frameBufferTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TextureParameter(_frameBufferTexture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(_frameBufferTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TextureParameter(_frameBufferTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _frameBufferTexture, 0);

            if (_rbo == 0)
                _rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, RenderSize.X, RenderSize.Y);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _frameBufferTexture, 0);
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
            if(DebugState.DebugLow)
                Scriptorium.Warn("Reloading all shader...");
            Shader.Reload();
        }

        internal static int AssignRendered(Renderable rendered)
        {
            if (_currentBindedPosition >= RenderElementsLimit)
                throw new IndexOutOfRangeException("Drawn mesh limit exceeded, either increase arbitrary limit or check for duplicated meshes");
            RenderPasses[_currentBindedPosition] = rendered.RenderPass;
            AllRender[_currentBindedPosition++] = rendered;
            
            _modifiedRendered = true;
            return _currentBindedPosition;
        }

        internal static void DisposeRendered(int bindPosition)
        {
            _modifiedRendered = true;
            AllRender[bindPosition] = null;
        }

        internal static void AssignSecondaryCamera(ViewportCamera viewportCamera)
        {
            if (_secondaryCamera != null)
            {
                if(DebugState.DebugMedium)
                    Scriptorium.Warn("A secondary camera is already assigned and yet another one is trying to replace it.");
                return;
            }
            _secondaryCamera = viewportCamera;
        }

        internal static void RemoveSecondaryCamera()
        {
            if (_secondaryCamera == null)
            {
                if(DebugState.DebugMedium)
                    Scriptorium.Warn("A path of execution is trying to remove a secondary camera when it is not yet assigned.");
                return;
            }
            _secondaryCamera = null;
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
            GuiRendering();
        }

        private static void RenderFramebuffer()
        {
            _fullscreenRender.Render(_frameBufferTexture, ScreenEffectTexture2D);
        }

        private static void RenderMeshed()
        {
            GL.ClearColor(Color.LightCyan);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            ArrangePasses();

            RenderingData data;
            //Main Camera
            //Usually, should not be null
            if (ViewportCamera.MainCamera != null)
            {
                data = new(ViewportCamera.MainCamera.Projection, ViewportCamera.MainCamera.View, ViewportCamera.MainCamera.Transform.Position, ViewportCamera.MainCamera.ViewFrustum);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                RenderPass(RenderingPass.geometryFirst, data);
                if (DebugState.DebugMedium)
                {
                    GL.Disable(EnableCap.DepthTest);
                    GL.Disable(EnableCap.CullFace);
                    DrawTemporal(data);
                    GL.Enable(EnableCap.DepthTest);
                    GL.Enable(EnableCap.CullFace);
                }
            }
            else if(DebugState.DebugMedium)
                Scriptorium.Warn("Main camera is null!");

            //Secondary Camera (if available)
            if (_secondaryCamera != null)
            {
                data = new(_secondaryCamera.Projection, _secondaryCamera.View, _secondaryCamera.Transform.Position, _secondaryCamera.ViewFrustum);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                RenderPass(RenderingPass.geometrySecond, data);
            }
            
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        private static void DrawTemporal(in RenderingData data)
        {
            foreach (var t in TemporalRenderables)
                t.Render(data);
            TemporalRenderables.Clear();
        }

        private static void ArrangePasses()
        {
            if (!_modifiedRendered)
                return;
            _modifiedRendered = false;
            GeometryFirst.Clear();
            GeometrySecond.Clear();
            Gui.Clear();
            for(int i = 0; i < _currentBindedPosition; i++)
            {
                if (AllRender[i] == null)
                    continue;
                switch (RenderPasses[i])
                {
                    case RenderingPass.geometryFirst:
                        GeometryFirst.Add(i);
                        break;
                    case RenderingPass.geometrySecond:
                        GeometrySecond.Add(i);
                        break;
                    case RenderingPass.userInterface:
                        Gui.Add(i);
                        break;
                }
            }
        }

        private static void RenderPass(RenderingPass pass, in RenderingData? data)
        {
            List<int> indexes = null!;
            switch (pass)
            {
                case RenderingPass.geometryFirst:
                    indexes = GeometryFirst;
                    break;
                case RenderingPass.geometrySecond:
                    indexes = GeometrySecond;
                    break;
                case RenderingPass.userInterface:
                    indexes = Gui;
                    break;
            }
            foreach(int index in indexes)
            {
                Renderable? r = AllRender[index];
                if (r == null || !r.AllowRendering)
                    continue;
                r.Render(pass ==  RenderingPass.geometryFirst ? data!.Value : default);
            }
        }

        private static void GuiRendering()
        {
            RenderPass(RenderingPass.userInterface, null);
        }

        public static void QueueTemporal(TemporalRenderable temp)
        {
            TemporalRenderables.Add(temp);
        }
    }
}
