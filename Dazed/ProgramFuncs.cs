using Dazed.Vitals;

namespace Dazed
{
    public class ProgramFuncs
    {
        //static void Main(string[] args)
        //{
        //    GameWindowSettings gSettings = new GameWindowSettings();
        //    NativeWindowSettings nSettings = new()
        //    {
        //        Title = "Dazed",
        //        WindowState = OpenTK.Windowing.Common.WindowState.Maximized
        //    };

        //    ProcessArguments(args);
        //    #if DEBUG
        //    if(!DebugState.DebugMedium)
        //        DebugState.Initialize(DebugLevel.Medium);
        //    #endif
        //    DazedInstance instance = new(gSettings, nSettings);
        //    instance.Run();
        //}

        public static void ProcessArguments(string[] args)
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
#if DEBUG
            if (!DebugState.DebugMedium)
                DebugState.Initialize(DebugLevel.Medium);
#endif
        }
    }
}
