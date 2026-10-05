using System.Reflection;

namespace Dazed.Vitals.Paisaje;
/// <summary>
/// The parts of the Paisaje that contain more logic than a <see cref="PaisajeGeometry"/>
/// </summary>
public abstract class PaisajeDynamic
{
    public Transform Transform { get; private set; }
    public string Name { get; internal set; } = null!;
    protected abstract void OnCreation();
    protected abstract void OnDestruction();
    
    public bool IsDestroyed { get; private set; }
    
    public static PaisajeDynamic Create(Type type, string name, ElementData data)
    {
        var rawObj = Activator.CreateInstance(type)!;
        ((PaisajeDynamic)rawObj).Name =  name;
        ((PaisajeDynamic)rawObj).InterpretData(type, data);
        ((PaisajeDynamic)rawObj).Transform = new Transform();
        ((PaisajeDynamic)rawObj).OnCreation();
        Phone.SubscribeToApplicationQuit(((PaisajeDynamic)rawObj).Destroy);
        return (PaisajeDynamic)rawObj;
    }

    internal void Destroy()
    {
        if (DebugState.DebugMedium) Scriptorium.Message($"Paisaje element of name <{Name}> Destroyed");
        Phone.UnSubscribeToApplicationQuit(Destroy);
        OnDestruction();
        IsDestroyed = true;
        GC.SuppressFinalize(this);
    }

    private void InterpretData(Type type, ElementData data)
    {
        for (var i = 0; i < data.Fields.Length; i++)
        {
            if (data.Fields[i].FieldName.StartsWith("param_"))
                InterpretAsParameter(data.Fields[i].FieldName, data.Data[data.Fields[i].ObjectFieldIndex], type);
            else
                InterpretAsField(data.Fields[i].FieldName, data.Data[data.Fields[i].ObjectFieldIndex], type);
        }
    }

    private void InterpretAsField(string dataField, object o, Type type)
    {
        FieldInfo? fieldInfo = type.GetField(dataField, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (fieldInfo == null)
        {
            if(DebugState.DebugMedium) Scriptorium.Message($"Field <{dataField}> not found, might have been deleted or renamed");
            return;
        }
        fieldInfo.SetValue(this, o);
    }

    private void InterpretAsParameter(string dataField, object o, Type type)
    {
        MethodInfo? methodInfo = type.GetMethod(dataField, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);
        if (methodInfo == null)
        {
            if(DebugState.DebugMedium)Scriptorium.Warn($"There is no set method with name <{dataField}>, it might have been deleted or renamed.");
            return;
        }
        methodInfo.Invoke(this, [o]);
    }

    public override string ToString()
    {
        return base.ToString() + $" Of name: {Name}";
    }
}