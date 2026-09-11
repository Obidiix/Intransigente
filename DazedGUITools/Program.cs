using System;

namespace DazedGUITools
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //TODO: Check for -mapbuild argument, as to allow the editor to be used for map compilation, transforming the .map file into a JSON file with all the required, precomupted data.
            var window = new MainWindow();
            window.Run();
            ImguiImplOpenGL3.Shutdown();
            ImguiImplOpenTK4.Shutdown();
        }
    }
}
