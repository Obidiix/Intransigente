using System.ComponentModel;
using System.Numerics;
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
        static System.Numerics.Vector3 movement = new();
        static Quaternion debugViewRotation = new();

        private static ViewportCamera DebugViewportCamera;
        private static IdentifiedAsset fontAsset;
        static float rotY;
        static Shader _testShader;
        static Material _testMaterial;
        static Mesh _testMesh;
        static Transform _testMeshTransform;
        static LocalizedString _testString;
        //static GuiText _testRenderText;
        static GuiButton _testButton;
        /// <summary>
        /// True when the game is ran to be played
        /// </summary>
        internal static bool IsContextRuntime { get; private set; }

        public static bool Quited { get; private set; }

        private readonly static KeyCombination _reloadShadersCombo = new(Keys.LeftControl, Keys.F8);
        private bool _triggeredReload;

        public DazedInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            IsContextRuntime = true;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Chest.Initialize();
            Artist.Initialize();
            Artist.SetResolution(ClientSize.X, ClientSize.Y);

            _testMeshTransform = new();
            fontAsset = new("defaultFont");
            Font.DefaultFont = Font.GetFontInstance(fontAsset);

            _testString = Locals.GetLocalizedString("test");
            //_testRenderText = new();
            //_testRenderText.Color = Color.Black;
            //_testRenderText.Text = _testString;
            //_testRenderText.SetPositionScreenRelative(0.5f, 0.5f);
            _testButton = new(_testString);
            _testButton.ImageColor = Color.Red;
            _testShader = Shader.GetShaderInstance("default.vert", "default.frag");
            _testMaterial = Material.GetMaterialInstance(_testShader);
            _testMesh = Mesh.GetMeshInstance(Chest.GetRelativePath("playerModel"), _testMaterial);
            DebugViewportCamera = new(90);
            Scriptorium.Message(_testString);

            UpdateFrequency = 60;
            VSync = VSyncMode.Adaptive;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Phone.Quit();
            Quited = true;
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

            if (Input.KeyJustPressed(Keys.Y))
            {
                Locals.SetLanguage(Locals.CurrentLanguage == Locals.AvailableLanguages[0] ? Locals.AvailableLanguages[1] : Locals.AvailableLanguages[0]);
                _testButton.Text = _testString;
            }
#if DEBUG
            if(Input.IsKeyDown(Keys.F10))
                Console.Clear();

            if (Input.IsKeyDown(Keys.G))
                _testButton.AddPosition(0, Chronos.DeltaTime);
            if (Input.IsKeyDown(Keys.H))
                _testButton.AddPosition(0, -Chronos.DeltaTime);
            if (Input.IsKeyDown(Keys.K))
                _testButton.AddPosition(Chronos.DeltaTime, 0);
            if (Input.IsKeyDown(Keys.L))
                _testButton.AddPosition(-Chronos.DeltaTime, 0);
#endif
            if (_reloadShadersCombo.KeysDown() && !_triggeredReload)
            {
                _triggeredReload = true;
                Artist.TriggerAllShadersReload();
            }
            if (_reloadShadersCombo.KeysReleased())
                _triggeredReload = false;
            
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
