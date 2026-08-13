using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals
{
    internal static class Input
    {
        private static KeyboardState _kState;
        private static MouseState _mState;

        internal static void UpdateInputState(KeyboardState keyboardState,  MouseState mouseState)
        {
            _kState = keyboardState;
            _mState = mouseState;
        }

        public static bool IsKeyDown(Keys key) => _kState.IsKeyDown(key);
        public static bool IsKeyDown(MouseButton button) => _mState.IsButtonDown(button);

        public static bool IsKeyUp(Keys key) => !_kState.IsKeyDown(key);
        public static bool IsKeyUp(MouseButton button) => !_mState.IsButtonDown(button);

        public static bool WasKeyReleased(Keys key) => _kState.IsKeyReleased(key);
        public static bool WasKeyReleased(MouseButton button) => _mState.IsButtonReleased(button);
    }
}
