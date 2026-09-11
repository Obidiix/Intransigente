using Newtonsoft.Json;

namespace Dazed.Vitals.Paisaje;

public readonly struct SerializedPaisaje
{
    [JsonProperty("b")] public readonly SerializedPaisajeElement[] Elements;
    [JsonProperty("c")] public readonly string[] Context;
    
    public SerializedPaisaje(SerializedPaisajeElement[] elements, string[] context)
    {
        Elements = elements;
        Context = context;
    }
}