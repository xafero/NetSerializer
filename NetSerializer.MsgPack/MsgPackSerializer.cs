using System;
using System.IO;
using MessagePack;
using NetSerializer.API;

// ReSharper disable InconsistentNaming

namespace NetSerializer.MsgPack
{
    public class MsgPackSerializer : ISerializer<string>
    {
        public MsgPackSerializer()
        {
            var resolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;
            MessagePackSerializer.SetDefaultResolver(resolver);
        }

        public string Serialize<O>(O input)
        {
            if (input == null) return string.Empty;
            using (var mem = new MemoryStream())
            {
                MessagePackSerializer.Serialize(mem, input);
                return Convert.ToBase64String(mem.ToArray());
            }
        }

        public O Deserialize<O>(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return default(O);
            using (var mem = new MemoryStream(Convert.FromBase64String(input)))
            {
                var raw = MessagePackSerializer.Deserialize<O>(mem);
                return raw;
            }
        }
    }
}