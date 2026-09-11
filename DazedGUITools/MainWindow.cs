using Dazed.Vitals;
using Dazed.Vitals.Content;
using DazedGUITools.Windows;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace DazedGUITools
{
    public class MainWindow : GameWindow
    {
        private static readonly List<WindowBaseClass> ActiveWindows = [];
        private static readonly HashSet<string> OccupiedWindowName = [];

        private static readonly List<WindowBaseClass> QueuedWindows = [];
        private static bool _isDrawing;

        private static readonly Type[] MainMenuWindows;
        private static readonly string[] MainMenuWindowsName;

        internal static string ProjectFolder => _projectFolder;
        private static string _projectFolder = "null";
        private static StartUpWindow? _startUpWindow;

        static MainWindow()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new();
            List<MainMenuWindowAttribute> attributes = new();
            for(int a = 0; a < assemblies.Length; a++)
            {
                List<Type> mW = GetMainMenuWindows(assemblies[a], out List<MainMenuWindowAttribute> atts);
                types.AddRange(mW);
                attributes.AddRange(atts);
            }

            MainMenuWindows = new Type[types.Count];
            MainMenuWindowsName = new string[types.Count];

            for(int i = 0; i < types.Count; i++)
            {
                MainMenuWindows[i] = types[i];
                MainMenuWindowsName[i] = attributes[i].Name;
            }
        }

        static List<Type> GetMainMenuWindows(Assembly assembly, out List<MainMenuWindowAttribute> atts)
        {
            atts = new();
            List<Type> windows = new();
            foreach (Type type in assembly.GetTypes())
            {
                object? att = type.GetCustomAttribute<MainMenuWindowAttribute>(true);
                if (att != null)
                {
                    atts.Add((MainMenuWindowAttribute)att);
                    windows.Add(type);
                }
            }
            return windows;
        }

        public MainWindow() : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(1600, 900), APIVersion = new Version(3, 3) })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Title = "Dazed GUI Tools";

            GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            ImGui.CreateContext();
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            ImGui.StyleColorsDark();

            ImGuiStylePtr style = ImGui.GetStyle();
            if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            {
                style.WindowRounding = 0.0f;
                style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
            }

            ImguiImplOpenTK4.Init(this);
            ImguiImplOpenGL3.Init();

            _startUpWindow = new();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            ImguiImplOpenGL3.NewFrame();
            ImguiImplOpenTK4.NewFrame();
            ImGui.NewFrame();

            ImGui.DockSpaceOverViewport();

            RenderGUI();
            
            ImGui.Render();
            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
            {
                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();
                Context.MakeCurrent();
            }

            SwapBuffers();
        }

        private static void RenderGUI()
        {
            DisplayMainMenuBar();

            if (_startUpWindow != null && _startUpWindow.MarkedForDisposal)
            {
                _projectFolder = _startUpWindow.ResultPath;
                _startUpWindow = null;
                GUIAppContext.Initialize();
            }

            if(QueuedWindows.Count > 0)
            {
                foreach (var win in QueuedWindows)
                    ActiveWindows.Add(win);
                QueuedWindows.Clear();
            }

            List<WindowBaseClass> disposedWindows = [];
            foreach(WindowBaseClass win in ActiveWindows)
            {
                _isDrawing = true;
                win.CallDraw();
                if (win.MarkedForDisposal)
                    disposedWindows.Add(win);
            }
            _isDrawing = false;
            if (disposedWindows.Count == 0)
                return;
            foreach (WindowBaseClass win in disposedWindows)
            {
                ActiveWindows.Remove(win);
                OccupiedWindowName.Remove(win._uniqueTitle);
            }
        }

        private static void DisplayMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Context Options"))
                {
                    if(ImGui.MenuItem("Save Content Table", "Ctrl + B (TODO)"))
                        SaveContentTable();
                    ImGui.EndMenu();
                }
                if(ImGui.BeginMenu("Tool Windows"))
                {
                    for(int i = 0; i < MainMenuWindowsName.Length; i++)
                    {
                        if (ImGui.MenuItem(MainMenuWindowsName[i]))
                        {
                            Activator.CreateInstance(MainMenuWindows[i]);
                        }
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private static void SaveContentTable()
        {
            SerializedContentTable cT = Chest.SerializeContentTable();
            string content = JsonConvert.SerializeObject(cT);
            string pathToFile = Path.Combine(_projectFolder, Chest.ContentTableFileName);
            if (!File.Exists(pathToFile))
                using (var f = File.Create(pathToFile)) { f.Dispose(); }
            File.WriteAllText(pathToFile, content);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        public readonly static DebugProc DebugProcCallback = Window_DebugProc;
        private static void Window_DebugProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr messagePtr, IntPtr userParam)
        {
            string message = Marshal.PtrToStringAnsi(messagePtr, length);

            bool showMessage;

            switch (source)
            {
                case DebugSource.DebugSourceApplication:
                    showMessage = false;
                    break;
                case DebugSource.DontCare:
                case DebugSource.DebugSourceApi:
                case DebugSource.DebugSourceWindowSystem:
                case DebugSource.DebugSourceShaderCompiler:
                case DebugSource.DebugSourceThirdParty:
                case DebugSource.DebugSourceOther:
                default:
                    showMessage = true;
                    break;
            }

            if (showMessage)
            {
                switch (severity)
                {
                    case DebugSeverity.DontCare:
                        Console.WriteLine($"[DontCare] [{source}] {message}");
                        break;
                    case DebugSeverity.DebugSeverityNotification:
                        break;
                    case DebugSeverity.DebugSeverityHigh:
                        Console.Error.WriteLine($"Error: [{source}] {message}");
                        break;
                    case DebugSeverity.DebugSeverityMedium:
                        Console.WriteLine($"Warning: [{source}] {message}");
                        break;
                    case DebugSeverity.DebugSeverityLow:
                        Console.WriteLine($"Info: [{source}] {message}");
                        break;
                    default:
                        Console.WriteLine($"[default] [{source}] {message}");
                        break;
                }
            }
        }

        internal static bool NameIsOccupied(string name) => OccupiedWindowName.Contains(name);

        internal static void AddWindow(WindowBaseClass window)
        {
            if (window == null)
                return;
            if (_isDrawing)
                QueuedWindows.Add(window);
            else
                ActiveWindows.Add(window);
            OccupiedWindowName.Add(window._uniqueTitle);
        }
    }
}
