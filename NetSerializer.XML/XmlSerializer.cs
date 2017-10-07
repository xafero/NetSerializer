using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public IDictionary<Type, Type> Mappings { get; }

        public XmlSerializer()
        {
            xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            serializers = new Dictionary<Type, Xmler>();
            WriteSettings = new XmlWriterSettings { OmitXmlDeclaration = true };
            Mappings = new Dictionary<Type, Type>
            {
                {typeof(TimeSpan), typeof(XmlTimeSpan)},
                {typeof(TimeSpan[]), typeof(XmlTimeSpan[])}
            };
        }

        private Xmler GetXmler(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
                type = typeof(XmlDictionary<,>).MakeGenericType(type.GetGenericArguments());
            Type rawType;
            if (!Mappings.TryGetValue(type, out rawType))
                if (type.IsGenericType)
                {
                    var genType = type.GetGenericTypeDefinition();
                    var found = false;
                    rawType = genType.MakeGenericType(type.GetGenericArguments().Select(t =>
                    {
                        var replaced = Mappings.TryGetValue(t, out rawType);
                        if (!replaced)
                            return t;
                        found = true;
                        return rawType;
                    }).ToArray());
                    if (found)
                        Mappings[type] = rawType;
                }
                else
                    rawType = type;
            Xmler xml;
            if (!serializers.TryGetValue(rawType, out xml))
                serializers[type] = xml = BuildSerializer(rawType);
            return xml;
        }

        public string Serialize<O>(O input)
        {
            var serializer = GetXmler(typeof(O));
            StringWriter writer;
            using (var xmlWriter = XmlWriter.Create(writer = new StringWriter(), WriteSettings))
            {
                var raw = ChangeConvert(input);
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
                var conv = ChangeConvert(raw);
                return (O)conv;
            }
        }

        private object ChangeConvert(object input)
        {
            ICollection coll;
            IDictionary dict;
            var raw = input;
            if (raw is TimeSpan)
                raw = (XmlTimeSpan)(TimeSpan)raw;
            else if (raw is TimeSpan[])
                raw = ((TimeSpan[])raw).Select(t => (XmlTimeSpan)t).ToArray();
            else if (raw is XmlTimeSpan)
                raw = (TimeSpan)(XmlTimeSpan)raw;
            else if (raw is XmlTimeSpan[])
                raw = ((XmlTimeSpan[])raw).Select(t => (TimeSpan)t).ToArray();
            else if ((dict = raw as IDictionary) != null)
            {
                var args = raw.GetType().GetGenericArguments();
                Type mappedKey;
                Type mappedVal;
                if (!TryGetValue(args.First(), out mappedKey)) mappedKey = args.First();
                if (!TryGetValue(args.Last(), out mappedVal)) mappedVal = args.Last();
                var mapped = typeof(XmlDictionary<,>).MakeGenericType(mappedKey, mappedVal);
                dynamic container = Activator.CreateInstance(mapped);
                foreach (dynamic item in dict)
                {
                    var rawKey = ChangeConvert(item.Key);
                    var rawVal = ChangeConvert(item.Value);
                    container.Add(rawKey, rawVal);
                }
                raw = container;
            }
            else if ((coll = raw as ICollection) != null)
            {
                Type mapped;
                if (TryGetValue(raw.GetType(), out mapped))
                {
                    dynamic container = Activator.CreateInstance(mapped);
                    foreach (var item in coll)
                    {
                        dynamic rawItem = ChangeConvert(item);
                        container.Add(rawItem);
                    }
                    raw = container;
                }
            }
            return raw;
        }

        private bool TryGetValue(Type raw, out Type mapped)
            => Mappings.TryGetValue(raw, out mapped)
               || (mapped = Mappings.FirstOrDefault(m => m.Value == raw).Key) != null;

        private Xmler BuildSerializer(Type rawType)
        {
            var propTypes = rawType.GetProperties().Select(p =>
            {
                Type mapped;
                return TryGetValue(p.PropertyType, out mapped) ? new { Prop = p, Map = mapped } : null;
            }).Where(p => p != null).ToArray();
            if (!propTypes.Any())
                return new Xmler(rawType);
            var overrides = new XmlAttributeOverrides();
            foreach (var pair in propTypes)
                overrides.Add(rawType, pair.Prop.Name, new XmlAttributes
                {
                    XmlElements = { new XmlElementAttribute(pair.Map) }
                });
            return new Xmler(rawType, overrides);
        }
    }
}