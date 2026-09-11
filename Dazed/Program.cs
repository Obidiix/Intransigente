using Dazed.Vitals;
using OpenTK.Windowing.Desktop;

namespace Dazed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gSettings = new GameWindowSettings();
            NativeWindowSettings nSettings = new()
            {
                Title = "Dazed",
                ClientSize = new(1280, 720)
            };
            ProcessArguments(args);
            #if DEBUG
            if(!DebugState.DebugMedium)
                DebugState.Initialize(DebugLevel.Medium);
            #endif
            DazedInstance instance = new(gSettings, nSettings);
            instance.Run();
        }

        private static void ProcessArguments(string[] args)
        {
            foreach (var argument in args)
            {
                if (argument.ToLower().StartsWith("-debug:"))
                {
                    DebugLevel level = DebugLevel.None;
                    if(argument.ToLower().EndsWith("low"))
                        level = DebugLevel.Low;
                    if(argument.ToLower().EndsWith("medium"))
                        level = DebugLevel.Medium;
                    if(argument.ToLower().EndsWith("full"))
                        level = DebugLevel.Full;
                    DebugState.Initialize(level);
                }
            }
        }
    }
}
