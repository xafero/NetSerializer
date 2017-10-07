using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetSerializer.API;

// ReSharper disable InconsistentNaming

namespace NetSerializer.Binary
{
    public class BinarySerializer : ISerializer<string>
    {
        private readonly BinaryFormatter formatter;

        public BinarySerializer()
        {
            formatter = new BinaryFormatter();
        }

        public string Serialize<O>(O input)
        {
            if (input == null) return string.Empty;
            using (var mem = new MemoryStream())
            {
                formatter.Serialize(mem, input);
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        public O Deserialize<O>(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return default(O);
            using (var mem = new MemoryStream(Convert.FromBase64String(input)))
            {
                var raw = formatter.Deserialize(mem);
                return (O) raw;
            }
        }
    }
}