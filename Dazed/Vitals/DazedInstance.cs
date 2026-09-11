using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;
using Dazed.Vitals.Rendering.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Dazed.Vitals
{
    public class DazedInstance : GameWindow
    {
        internal static Matrix4 debugViewMatrix => debugCamera.View;
        internal static Matrix4 debugProjectionMatrix => debugCamera.Projection;
        internal static Vector3 debugCameraPosition => (Vector3)debugCamera.Transform.Position;
        static System.Numerics.Vector3 movement = new();
        static Quaternion debugViewRotation = new();

        internal static Camera debugCamera;
        internal static IdentifiedAsset fontAsset;
        internal static Font testFont;
        static float rotY;
        static Shader _testShader;
        static Material _testMaterial;
        static Mesh _testMesh;
        static Transform _testMeshTransform;
        internal static bool IsContextRuntime => _isContextRuntime;

        private static bool _isContextRuntime;

        private static KeyCombination _reloadShadersCombo = new(Keys.LeftControl, Keys.F8);
        private bool _triggeredReload;

        public DazedInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _isContextRuntime = true;
        }

        static DazedInstance()
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Chest.Initialize();
            Artist.Initialize();
            Artist.SetResolution(ClientSize.X, ClientSize.Y);

            _testMeshTransform = new();
            fontAsset = new("defaultFont");
            testFont = Font.GetFontInstance(fontAsset);
            _testShader = Shader.GetShaderInstance("default.vert", "default.frag");
            _testMaterial = Material.GetMaterialInstance(_testShader);
            _testMesh = Mesh.GetMeshInstance(Chest.GetRelativePath("playerModel"), _testMaterial);
            debugCamera = new(90);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
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
            movement = (System.Numerics.Vector3)Vector3.Zero;
            if (Input.IsKeyDown(Keys.W))
                movement.Z += -Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.S))
                movement.Z += Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.D))
                movement.X += Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.A))
                movement.X += -Chronos.DeltaTime;
            debugCamera.Move(movement, Chronos.DeltaTime);

            if (Input.IsKeyDown(Keys.Left))
                rotY += -MathHelper.DegreesToRadians(Chronos.DeltaTime * 10);
            if (Input.IsKeyDown(Keys.Right))
                rotY += MathHelper.DegreesToRadians(Chronos.DeltaTime * 10);
            debugViewRotation = Quaternion.FromAxisAngle(Vector3.UnitY, rotY);
            debugCamera.Transform.Rotation = debugViewRotation;

#if DEBUG
            if(Input.IsKeyDown(Keys.F10))
                Console.Clear();
#endif
            if (_reloadShadersCombo.KeysDown() && !_triggeredReload)
            {
                _triggeredReload = true;
                Artist.TriggerAllShadersReload();
            }
            if (_reloadShadersCombo.KeysReleased())
                _triggeredReload = false;
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _testMesh.ApplyRelativeToTransform(_testMeshTransform.World);
            debugCamera.PreDrawCall();
            Artist.DrawFrame(args);
            SwapBuffers();
        }
    }
}
