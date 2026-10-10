using Newtonsoft.Json;

namespace Dazed.Vitals.AssetReference
{
    public readonly struct ExposedDataArrays
    {
        [JsonProperty("f")]
        public readonly string[] ClassAndField;
        [JsonProperty("d")]
        public readonly object?[] FieldData;

        [JsonConstructor]
        public ExposedDataArrays(string[] classAndField, object?[] fieldData)
        {
            ClassAndField = classAndField;
            FieldData = fieldData;
        }
    }
}
