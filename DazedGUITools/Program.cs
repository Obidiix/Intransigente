namespace DazedGUITools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var window = new MainWindow();
            window.Run();
            ImguiImplOpenGL3.Shutdown();
            ImguiImplOpenTK4.Shutdown();
        }
    }
}
