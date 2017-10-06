using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NetSerializer.API;
using Xmler = System.Xml.Serialization.XmlSerializer;

// ReSharper disable InconsistentNaming

namespace NetSerializer.XML
{
    public class XmlSerializer : ISerializer<string>
    {
        private readonly XmlSerializerNamespaces xns;
        private readonly IDictionary<Type, Xmler> serializers;

        public XmlWriterSettings WriteSettings { get; }

        public XmlSerializer()
        {
            xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            serializers = new Dictionary<Type, Xmler>();
            WriteSettings = new XmlWriterSettings {OmitXmlDeclaration = true};
        }

        private Xmler GetXmler(Type type)
        {
            var rawType = type;
            if (rawType == typeof(TimeSpan))
                rawType = typeof(XmlTimeSpan);
            Xmler xml;
            if (!serializers.TryGetValue(rawType, out xml))
                serializers[type] = xml = new Xmler(rawType);
            return xml;
        }

        public string Serialize<O>(O input)
        {
            var serializer = GetXmler(typeof(O));
            StringWriter writer;
            using (var xmlWriter = XmlWriter.Create(writer = new StringWriter(), WriteSettings))
            {
                object raw = input;
                if (raw is TimeSpan)
                    raw = (XmlTimeSpan) (TimeSpan) raw;
                serializer.Serialize(xmlWriter, raw, xns);
                return writer.ToString();
            }
        }

        public O Deserialize<O>(string input)
        {
            var serializer = GetXmler(typeof(O));
            using (var reader = new StringReader(input))
            {
                var raw = serializer.Deserialize(reader);
                if (raw is XmlTimeSpan)
                    raw = (TimeSpan) (XmlTimeSpan) raw;
                return (O) raw;
            }
        }
    }
}