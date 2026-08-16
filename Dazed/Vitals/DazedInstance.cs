using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;
using Dazed.Vitals.Rendering.TextRendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
        internal static Font testFont;
        internal static RenderedText rText;
        static float rotY;

        internal static bool IsContextRuntime => _isContextRuntime;
        private static bool _isContextRuntime;

        public DazedInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _isContextRuntime = true;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Chest.Initialize();
            Artist.Initialize();
            Artist.SetResolution(Size.X, Size.Y);

            testFont = new(Chest.GetRelativePath("defaultFont"));
            rText = new(testFont);
            rText.Text = "Pato ñato";
            rText.Color = Color4.Red; 
            debugCamera = new(90);
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            Artist.SetResolution(e.Width, e.Height);
            Camera.ReloadAllProjections();
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
            debugCamera.Transform.Rotation = (System.Numerics.Quaternion)debugViewRotation;

            if (Input.IsKeyDown(Keys.Up))
                rText.Size += 0.1f;
            if (Input.IsKeyDown(Keys.Down))
                rText.Size -= 0.1f;
            if (Input.IsKeyDown(Keys.Right))
                rText.HorizontalSpacing += Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.Left))
                rText.HorizontalSpacing -= Chronos.DeltaTime;

            if (Input.IsKeyDown(Keys.L))
                rText.VerticalSpacing += Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.K))
                rText.VerticalSpacing -= Chronos.DeltaTime;

            if (Input.IsKeyDown(Keys.P))
                rText.Position += Vector2.UnitX * Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.O))
                rText.Position -= Vector2.UnitX * Chronos.DeltaTime;

            if (Input.IsKeyDown(Keys.D0))
                rText.Position += Vector2.UnitY * Chronos.DeltaTime;
            if (Input.IsKeyDown(Keys.D9))
                rText.Position -= Vector2.UnitY * Chronos.DeltaTime;
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            debugCamera.PreDrawCall();
            Artist.UpdateFrame(args);
            rText.Render();
            SwapBuffers();
        }
    }
}
