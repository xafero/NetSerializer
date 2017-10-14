using NetSerializer.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming

namespace NetSerializer.JSON
{
    public class JsonSerializer : ISerializer<string>
    {
        public JsonSerializerSettings Settings { get; }

        public JsonSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = {new StringEnumConverter()}
            };
        }

        public string Serialize<O>(O input)
        {
            var json = JsonConvert.SerializeObject(input, Settings);
            return json;
        }

        public O Deserialize<O>(string input)
        {
            var obj = JsonConvert.DeserializeObject<O>(input, Settings);
            return obj;
        }
    }
}