using System;
using NetSerializer.API;
using NetSerializer.JSON;
using NetSerializer.XML;
using NetSerializer.YAML;
using NUnit.Framework;

namespace NetSerializer.Test
{
    [TestFixture]
    public class SerializerTests
    {
        [Test]
        public void ShouldYamlSerialize() => TestSerialize(new YamlSerializer());

        [Test]
        public void ShouldJsonSerialize() => TestSerialize(new JsonSerializer());

        [Test]
        public void ShouldXmlSerialize() => TestSerialize(new XmlSerializer());

        private static void TestSerialize(ISerializer<string> serializer)
        {
            TestSerialize<byte>(serializer, 127);
            TestSerialize<sbyte>(serializer, -128);
            TestSerialize(serializer, 42);
            TestSerialize(serializer, 42u);
            TestSerialize<short>(serializer, 42);
            TestSerialize<ushort>(serializer, 42);
            TestSerialize(serializer, 42L);
            TestSerialize(serializer, 42ul);
            TestSerialize(serializer, 42.1f);
            TestSerialize(serializer, 42.12);
            TestSerialize(serializer, 'b');
            TestSerialize(serializer, true);
            TestSerialize<object>(serializer, null);
            TestSerialize(serializer, "ab");
            TestSerialize(serializer, 42.12m);
            TestSerialize(serializer, DateTime.Now);
            TestSerialize(serializer, TimeSpan.FromMinutes(5));
        }

        private static void TestSerialize<T>(ISerializer<string> serializer, T input)
        {
            var text = serializer.Serialize(input);
            Assert.AreEqual(input, serializer.Deserialize<T>(text), text);
            Console.WriteLine($"[{input?.GetType().Name}] {CleanUp(text)}");
        }

        private static string CleanUp(string text)
            => text.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
    }
}