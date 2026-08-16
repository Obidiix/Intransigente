using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Content
{
    public class SerializedContentTable
    {
        [JsonProperty("k")]
        private readonly string[] keys;
        [JsonProperty("p")]
        private readonly string[] paths;

        [JsonConstructor]
        public SerializedContentTable(string[] keys, string[] paths)
        {
            this.keys = keys;
            this.paths = paths;
        }

        [JsonIgnore]
        public string[] Keys => keys;
        [JsonIgnore]
        public string[] Paths => paths;
    }
}
