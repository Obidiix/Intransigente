using Dazed;
using Dazed.Vitals;
using OpenTK.Windowing.Desktop;

namespace Intransigente
{
    internal class Program
    {
        private const int SecondsForProgramAnswer = 10;
        private static DazedInstance _instance;
        private static bool _end;

        static void Main(string[] args)
        {
            GameWindowSettings gSettings = new GameWindowSettings();
            NativeWindowSettings nSettings = new()
            {
                Title = "Intransigente",
                MinimumClientSize = new OpenTK.Mathematics.Vector2i(320, 240),
                AspectRatio = (640, 480),
                WindowState = OpenTK.Windowing.Common.WindowState.Maximized
            };

            ProgramFuncs.ProcessArguments(args);
            Scriptorium.OnEntryRecord += Log;
            _instance = new(gSettings, nSettings);
            Task exitTask = new(LifeCheck);
            exitTask.Start();
            _instance.Run();
            _end = true;
            exitTask.Wait();
            exitTask.Dispose();
            Scriptorium.OnEntryRecord -= Log;
        }

        private static void LifeCheck()
        {
            if (DazedInstance.LastUpdateCall.ElapsedMilliseconds >= SecondsForProgramAnswer * 1000)
            {
                Scriptorium.Warn($"It's been {SecondsForProgramAnswer}s since last update from the game, and so it was terminated.");
                Environment.Exit(-1);
            }
            else
            {
                Thread.Sleep(100);
                if (_end)
                    return;
                LifeCheck();
            }
        }

        private static void Log(string message)
        {

        }
    }
}
