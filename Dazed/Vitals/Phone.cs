namespace Dazed.Vitals;
/// <summary>
/// Signals attached classes and calls the methods they subscribe to, in a certain moment
/// </summary>
internal static class Phone
{
    private static event Action? UpdateCall;
    private static event Action? DrawCall;
    private static event Action? QuitCall;
    
    public static void SubscribeToUpdate(Action subscribed) => UpdateCall += subscribed;
    public static void UnSubscribeToUpdate(Action subscribed) => UpdateCall -= subscribed;
    
    public static void SubscribeToDrawCall(Action subscribed) => DrawCall += subscribed;
    public static void UnSubscribeToDrawCall(Action subscribed) => DrawCall -= subscribed;
    
    public static void SubscribeToApplicationQuit(Action subscribed) => QuitCall += subscribed;
    public static void UnSubscribeToApplicationQuit(Action subscribed) => QuitCall -= subscribed;
    
    internal static void Update() => UpdateCall?.Invoke();
    internal static void Draw() => DrawCall?.Invoke();
    internal static void Quit() => QuitCall?.Invoke();
}