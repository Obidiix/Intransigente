namespace Dazed.Vitals.Paisaje;
/// <summary>
/// The parts of the Paisaje that contain more logic than a <see cref="PaisajeBrush"/>
/// </summary>
public abstract class PaisajeElement
{
    public string Name { get; internal set; } = null!;
    protected abstract void OnCreation();
    protected abstract void OnDestruction();

    public static PaisajeElement Create(Type type, string name, object[] data)
    {
        var rawObj = Activator.CreateInstance(type)!;
        ((PaisajeElement)rawObj).Name =  name;
        ((PaisajeElement)rawObj).OnCreation();
        Phone.SubscribeToApplicationQuit(((PaisajeElement)rawObj).Destroy);
        //TODO: Process data and pass to element
        return (PaisajeElement)rawObj;
    }

    internal void Destroy()
    {
        if (DebugState.DebugFull) Scriptorium.Message($"Paisaje element of name <{Name}> Destroyed");
        Phone.UnSubscribeToApplicationQuit(Destroy);
        OnDestruction();
    }

    public override string ToString()
    {
        return base.ToString() + $" Of name: {Name}";
    }
}