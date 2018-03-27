using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Binary;

namespace Vostok.Context.Tests
{
    [TestFixture]
    public class FlowingContext_Tests
    {
        private const string Int_123 = "AgAAAAMAAABrZXkMAAAAU3lzdGVtLkludDMyewAAAAUAAABjbGFzczEAAABWb3N0b2suQ29udGV4dC5UZXN0cy5GbG93aW5nQ29udGV4dF9UZXN0cytNeUNsYXNzewAAAAQAAAB0ZXh0";
        private const string Custom_123_text = "AgAAAAMAAABrZXkMAAAAU3lzdGVtLkludDMyAgAAAAUAAABjbGFzczEAAABWb3N0b2suQ29udGV4dC5UZXN0cy5GbG93aW5nQ29udGV4dF9UZXN0cytNeUNsYXNzewAAAAQAAAB0ZXh0";

        [Test]
        public void Should_overwrite_mode_work_correctly()
        {
            new Action(() =>
            {
                FlowingContext.Set("key", 1);
                FlowingContext.Set("key", 2);
            }).Should().Throw<ArgumentException>();

            new Action(() =>
            {
                FlowingContext.SetOverwriteMode(true);
                FlowingContext.Set("key", 2);
            }).Should().NotThrow<ArgumentException>();
        }

        [Test]
        public void Should_serialize_simple_type()
        {
            FlowingContext.Set("key", 123);
            FlowingContext.Serialize().Should().Be(Int_123);
        }

        [Test]
        public void Should_deserialize_simple_type()
        {
            FlowingContext.Deserialize(Int_123);
            FlowingContext.Get<int>("key").Should().Be(123);
        }

        private class MyClass
        {
            public int Int;
            public string String;
        }

        [Test]
        public void Should_serialize_custom_type()
        {
            var val = new MyClass{ Int = 123, String = "text" };

            FlowingContext.SetSerializer((MyClass value, IBinaryWriter writer) =>
            {
                writer.Write(value.Int);
                writer.Write(value.String);
            });
            FlowingContext.Set("class", val);
            FlowingContext.Serialize().Should().Be(Custom_123_text);
        }

        [Test]
        public void Should_deserialize_custom_type()
        {
            FlowingContext.SetDeserializer(reader => new MyClass
            {
                Int = reader.ReadInt32(),
                String = reader.ReadString()
            });
            FlowingContext.Deserialize(Custom_123_text);
            FlowingContext.Get<MyClass>("class").Should().BeEquivalentTo(new MyClass { Int = 123, String = "text" });
        }
    }
}