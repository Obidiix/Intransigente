using Dazed.Vitals;
using OpenTK.Windowing.Desktop;

namespace Dazed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gSettings = new GameWindowSettings();
            NativeWindowSettings nSettings = new() { Title = "Dazed" };
            DazedInstance instance = new(gSettings, nSettings);
            instance.Run();
        }
    }
}
