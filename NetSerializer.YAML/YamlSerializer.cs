using NetSerializer.API;
using YamlDotNet.Serialization;

// ReSharper disable InconsistentNaming

namespace NetSerializer.YAML
{
    public class YamlSerializer : ISerializer<string>
    {
        public Serializer Serializer { get; }
        public Deserializer Deserializer { get; }

        public YamlSerializer()
        {
            var sbld = new SerializerBuilder();
            Serializer = sbld.Build();
            var dbld = new DeserializerBuilder();
            Deserializer = dbld.Build();
        }

        public string Serialize<O>(O input)
        {
            var yaml = Serializer.Serialize(input);
            return yaml;
        }

        public O Deserialize<O>(string input)
        {
            var obj = Deserializer.Deserialize<O>(input);
            return obj;
        }
    }
}