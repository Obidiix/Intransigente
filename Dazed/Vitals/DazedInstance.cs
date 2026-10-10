using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Dazed.Vitals.AssetReference;
using Dazed.Vitals.Content;
using Dazed.Vitals.Localization;
using Dazed.Vitals.Paisaje;
using Dazed.Vitals.Rendering;
using Dazed.Vitals.Rendering.GUI;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Dazed.Vitals
{
    public class DazedInstance : GameWindow
    {
        static Vector3 movement = new();
        static Quaternion debugViewRotation = new();

        public static Stopwatch LastUpdateCall { get; private set; } = new();

        private static ViewportCamera DebugViewportCamera;
        private static IdentifiedAsset fontAsset;
        static float rotY;
        static Shader _testShader;
        static Material _testMaterial;
        [ExposedField("playerMesh")]
        private AssetPath _playerMeshPath;
        static Mesh _testMesh;
        static Transform _testMeshTransform;
        /// <summary>
        /// True when the game is ran to be played
        /// </summary>
        internal static bool IsContextRuntime { get; private set; }

        public static bool Quited { get; private set; }

        private readonly static KeyCombination _reloadShadersCombo = new(Keys.LeftControl, Keys.F8);

        public DazedInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            MaximumSize = new(CurrentMonitor.HorizontalResolution, CurrentMonitor.VerticalResolution);
            ClientSize = MaximumSize.Value;
            AlwaysOnTop = false;
            IsContextRuntime = true;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            ExposedData.RetrieveAll(GetType(), this);
            Chest.Initialize();
            Artist.Initialize();
            Artist.SetResolution(ClientSize.X, ClientSize.Y);

            _testMeshTransform = new();
            fontAsset = new("defaultFont");
            Font.DefaultFont = Font.GetFontInstance(fontAsset);

            _testShader = Shader.GetShaderInstance("default.vert", "default.frag");
            _testMaterial = Material.GetMaterialInstance(_testShader);
            _testMesh = Mesh.GetMeshInstance(_playerMeshPath.AssetRelativePath, _testMaterial);
            DebugViewportCamera = new(90);

            UpdateFrequency = CurrentMonitor.CurrentVideoMode.RefreshRate;
            VSync = VSyncMode.Adaptive;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Phone.Quit();
            Quited = true;
            LastUpdateCall.Stop();
            base.OnClosing(e);
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            Artist.SetResolution(e.Width, e.Height);
            Artist.ReloadWindowPosition(ClientLocation);
        }

        protected override void OnMove(WindowPositionEventArgs e)
        {
            base.OnMove(e);
            Artist.ReloadWindowPosition(ClientLocation);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            LastUpdateCall.Start();
            base.OnUpdateFrame(args);
            Chronos.UpdateDeltaTime(Convert.ToSingle(args.Time));
            Input.UpdateInputState(KeyboardState, MouseState);
            movement = Vector3.Zero;
            if (Input.IsKeyDown(Keys.W))
                movement.Z += -1;
            if (Input.IsKeyDown(Keys.S))
                movement.Z += 1;
            if (Input.IsKeyDown(Keys.D))
                movement.X += 1;
            if (Input.IsKeyDown(Keys.A))
                movement.X += -1;
            if (Input.IsKeyDown(Keys.E))
                movement.Y += 1;
            if (Input.IsKeyDown(Keys.Q))
                movement.Y += -1;
            DebugViewportCamera.Move(movement, Chronos.DeltaTime);

            if (Input.IsKeyDown(Keys.Left))
                rotY += -MathTeacher.DegreesToRadians * Chronos.DeltaTime * 50;
            if (Input.IsKeyDown(Keys.Right))
                rotY += MathTeacher.DegreesToRadians * Chronos.DeltaTime * 50;
            debugViewRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotY);
            DebugViewportCamera.Transform.Rotation = debugViewRotation;
#if DEBUG
            if(Input.IsKeyDown(Keys.F10))
                Console.Clear();

            if (Input.IsKeyDown(Keys.I))
                _testMeshTransform.MoveGlobal(Vector3.UnitZ * Chronos.DeltaTime);
            if (Input.IsKeyDown(Keys.J))
                _testMeshTransform.MoveGlobal(-Vector3.UnitX * Chronos.DeltaTime);
            if (Input.IsKeyDown(Keys.K))
                _testMeshTransform.MoveGlobal(-Vector3.UnitZ * Chronos.DeltaTime);
            if (Input.IsKeyDown(Keys.L))
                _testMeshTransform.MoveGlobal(Vector3.UnitX * Chronos.DeltaTime);
#endif
            if (_reloadShadersCombo.FirstHoldSecondPressed())
                Artist.TriggerAllShadersReload();
            if (Input.KeyJustPressed(Keys.F11))
            {
                switch (WindowState)
                {
                    case WindowState.Fullscreen:
                        WindowState = WindowState.Maximized;
                        break;
                    default:
                        WindowState = WindowState.Fullscreen;
                        break;
                }
            }

            LastUpdateCall.Restart();
            Phone.Update();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            if(Quited)
                return;
            Mundo.DrawPaisaje(DebugViewportCamera.ViewFrustum);
            Phone.Draw();
            _testMesh.ApplyRelativeToTransform(_testMeshTransform.World);
            Artist.DrawFrame(args);
            SwapBuffers();
        }
    }
}
