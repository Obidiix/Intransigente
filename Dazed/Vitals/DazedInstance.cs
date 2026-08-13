using Dazed.Vitals.Content;
using Dazed.Vitals.Rendering;
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

        private static Camera debugCamera;
        static float rotY;

        public DazedInstance(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Chest.Initialize();
            Artist.Initialize();
            Artist.SetResolution(Size.X, Size.Y);

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

            if (Input.WasKeyReleased(Keys.Up))
                Artist.RenderingScale += 0.1f;
            if (Input.WasKeyReleased(Keys.Down))
                Artist.RenderingScale -= 0.1f;
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            debugCamera.PreDrawCall();
            Artist.UpdateFrame(args);
            SwapBuffers();
        }
    }
}
