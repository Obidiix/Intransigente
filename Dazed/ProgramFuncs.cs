using Dazed.Vitals;

namespace Dazed
{
    public static class ProgramFuncs
    {
        public static bool StoreLogs { get; private set; }
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
                else if(argument.ToLower() == "-logall")
                    StoreLogs = true;
            }
#if DEBUG
            if (!DebugState.DebugMedium)
                DebugState.Initialize(DebugLevel.Medium);
#endif
        }
        private const string DataOriginFileName = "dataorigin.txt";
        public static void RetrieveDataFiles()
        {
            string path = PathUtils.BaseDirectoryCombine(DataOriginFileName);
            if (!File.Exists(path))
                return;
            string content = File.ReadAllLines(path).First();
            if (!Directory.Exists(content))
                return;
            foreach (string dirPath in Directory.GetDirectories(content, "*", SearchOption.AllDirectories))
            {
                string pathTo = dirPath.Replace(content, PathUtils.BaseDirectory);
                if (Directory.Exists(pathTo))
                    continue;
                Directory.CreateDirectory(pathTo);
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string filePath in Directory.GetFiles(content, "*.*", SearchOption.AllDirectories))
            {
                if (filePath.EndsWith(".ignore"))
                    continue;
                string pathTo = filePath.Replace(content, PathUtils.BaseDirectory);
                if (File.Exists(pathTo))
                {
                    var last = File.GetLastWriteTimeUtc(pathTo);
                    DateTime fileLastWrite = File.GetLastWriteTimeUtc(filePath);
                    if (last >= fileLastWrite)
                        continue;
                }
                File.Copy(filePath, pathTo, true);
            }
        }
    }
}
