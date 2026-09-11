namespace Dazed.Vitals;

public static class DebugState
{
    private static DebugLevel DebugLevel { get; set; }
    public static bool DebugLow => DebugLevel >= DebugLevel.Low;
    public static bool DebugMedium => DebugLevel >= DebugLevel.Medium;
    public static bool DebugFull => DebugLevel >= DebugLevel.Full;
    
    internal static void Initialize(DebugLevel level)
    {
        DebugLevel =  level;
    }
}