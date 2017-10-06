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
        public void ShouldYamlSerialize()
        {
            var seri = new YamlSerializer();
            TestPrimSerialize(seri);
            TestArraySerialize(seri);
        }

        [Test]
        public void ShouldJsonSerialize()
        {
            var seri = new JsonSerializer();
            TestPrimSerialize(seri);
            TestArraySerialize(seri);
        }

        [Test]
        public void ShouldXmlSerialize()
        {
            var seri = new XmlSerializer();
            TestPrimSerialize(seri);
            TestArraySerialize(seri);
        }

        private static void TestPrimSerialize(ISerializer<string> serializer)
        {
            TestPrimSerialize<byte>(serializer, 127);
            TestPrimSerialize<sbyte>(serializer, -128);
            TestPrimSerialize(serializer, 42);
            TestPrimSerialize(serializer, 42u);
            TestPrimSerialize<short>(serializer, 42);
            TestPrimSerialize<ushort>(serializer, 42);
            TestPrimSerialize(serializer, 42L);
            TestPrimSerialize(serializer, 42ul);
            TestPrimSerialize(serializer, 42.1f);
            TestPrimSerialize(serializer, 42.12);
            TestPrimSerialize(serializer, 'b');
            TestPrimSerialize(serializer, true);
            TestPrimSerialize<object>(serializer, null);
            TestPrimSerialize(serializer, "ab");
            TestPrimSerialize(serializer, 42.12m);
            TestPrimSerialize(serializer, DateTime.Now);
            TestPrimSerialize(serializer, TimeSpan.FromMinutes(5));
        }

        private static void TestPrimSerialize<T>(ISerializer<string> serializer, T input)
        {
            var text = serializer.Serialize(input);
            Assert.AreEqual(input, serializer.Deserialize<T>(text), text);
            Console.WriteLine($"[{input?.GetType().Name}] {CleanUp(text)}");
        }

        private static void TestArraySerialize(ISerializer<string> serializer)
        {
            TestArraySerialize<byte>(serializer, 127, 128);
            TestArraySerialize<sbyte>(serializer, -128, -127);
            TestArraySerialize(serializer, 42, 43);
            TestArraySerialize(serializer, 42u, 43u);
            TestArraySerialize<short>(serializer, 42, 43);
            TestArraySerialize<ushort>(serializer, 42, 43);
            TestArraySerialize(serializer, 42L, 43L);
            TestArraySerialize(serializer, 42ul, 43ul);
            TestArraySerialize(serializer, 42.1f, 43.1f);
            TestArraySerialize(serializer, 42.12, 45.23);
            TestArraySerialize(serializer, 'b', 'a');
            TestArraySerialize(serializer, true, false);
            TestArraySerialize<object>(serializer, null, null);
            TestArraySerialize(serializer, "ab", "be");
            TestArraySerialize(serializer, 42.12m, 43.13m);
            TestArraySerialize(serializer, DateTime.Now, DateTime.Today);
            TestArraySerialize(serializer, TimeSpan.FromMinutes(5), TimeSpan.FromDays(3));
        }

        private static void TestArraySerialize<T>(ISerializer<string> serializer, params T[] input)
        {
            var text = serializer.Serialize(input);
            Assert.AreEqual(input, serializer.Deserialize<T[]>(text), text);
            Console.WriteLine($"[{input?.GetType().Name}] {CleanUp(text)}");
        }

        private static string CleanUp(string text)
            => text.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
    }
}