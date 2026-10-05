using Newtonsoft.Json;

namespace Dazed.Vitals;

public static class Cereal
{
    public static void SaveData<T>(T data, string absolutePath)
    {
        var json = JsonConvert.SerializeObject(data);
        if(!File.Exists(absolutePath))
            using (var f = File.Create(absolutePath)){}
        if (DebugState.DebugFull)
            Scriptorium.Warn($"File saved at path: {absolutePath} | Of type: {typeof(T)}");
        File.WriteAllText(absolutePath, json);
    }

    public static T LoadData<T>(string absolutePath)
    {
        var json = File.ReadAllText(absolutePath);
        if(DebugState.DebugFull)
            Scriptorium.Warn($"File loaded at path: {absolutePath} | Of type: {typeof(T)}");
        return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException();
    }
}