using System;
using System.Linq;
using System.Net;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace Vostok.Context.Tests
{
    [TestFixture]
    internal class ContextSerializers_Tests
    {
        [Test]
        public void Should_have_fields_for_all_implemented_serializers()
        {
            var serializerTypes =
                typeof(IContextSerializer<>).Assembly.GetTypes()
                    .Where(
                        t => t.GetInterfaces()
                                 .Where(i => i.IsGenericType)
                                 .Where(i => !i.ContainsGenericParameters)
                                 .Select(i => i.GetGenericTypeDefinition())
                                 .FirstOrDefault() == typeof(IContextSerializer<>));

            var serializerTypeNames = serializerTypes.Select(t => t.Name.Replace("Serializer", ""));

            var declaredFieldNames = typeof(ContextSerializers)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(f => f.Name);

            declaredFieldNames.Should().BeEquivalentTo(serializerTypeNames);
        }

        [TestCase("")]
        [TestCase("Hello!")]
        [TestCase("Привет!")]
        public void Should_correctly_serialize_and_deserialize_string_values(string value)
        {
            TestSerialization(value, ContextSerializers.String);
        }

        [TestCase(char.MinValue)]
        [TestCase(char.MaxValue)]
        [TestCase('X')]
        public void Should_correctly_serialize_and_deserialize_char_values(char value)
        {
            TestSerialization(value, ContextSerializers.Char);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Should_correctly_serialize_and_deserialize_bool_values(bool value)
        {
            TestSerialization(value, ContextSerializers.Bool);
        }

        [TestCase(byte.MaxValue)]
        [TestCase(byte.MinValue)]
        public void Should_correctly_serialize_and_deserialize_byte_values(byte value)
        {
            TestSerialization(value, ContextSerializers.Byte);
        }

        [TestCase(sbyte.MaxValue)]
        [TestCase(sbyte.MinValue)]
        public void Should_correctly_serialize_and_deserialize_signed_byte_values(sbyte value)
        {
            TestSerialization(value, ContextSerializers.SignedByte);
        }

        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        public void Should_correctly_serialize_and_deserialize_short_values(short value)
        {
            TestSerialization(value, ContextSerializers.Short);
        }

        [TestCase(ushort.MaxValue)]
        [TestCase(ushort.MinValue)]
        public void Should_correctly_serialize_and_deserialize_unsigned_short_values(ushort value)
        {
            TestSerialization(value, ContextSerializers.UnsignedShort);
        }

        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void Should_correctly_serialize_and_deserialize_int_values(int value)
        {
            TestSerialization(value, ContextSerializers.Int);
        }

        [TestCase(uint.MaxValue)]
        [TestCase(uint.MinValue)]
        public void Should_correctly_serialize_and_deserialize_unsigned_int_values(uint value)
        {
            TestSerialization(value, ContextSerializers.UnsignedInt);
        }

        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        public void Should_correctly_serialize_and_deserialize_long_values(long value)
        {
            TestSerialization(value, ContextSerializers.Long);
        }

        [TestCase(ulong.MaxValue)]
        [TestCase(ulong.MinValue)]
        public void Should_correctly_serialize_and_deserialize_unsigned_long_values(ulong value)
        {
            TestSerialization(value, ContextSerializers.UnsignedLong);
        }

        [TestCase(1.50F)]
        [TestCase(-1.55F)]
        [TestCase(float.Epsilon)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Should_correctly_serialize_and_deserialize_float_values(float value)
        {
            TestSerialization(value, ContextSerializers.Float);
        }

        [TestCase(1.50D)]
        [TestCase(-1.55D)]
        [TestCase(double.Epsilon)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Should_correctly_serialize_and_deserialize_double_values(double value)
        {
            TestSerialization(value, ContextSerializers.Double);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_decimal_values()
        {
            TestSerialization(decimal.MinValue, ContextSerializers.Decimal);
            TestSerialization(decimal.MaxValue, ContextSerializers.Decimal);
            TestSerialization(decimal.One, ContextSerializers.Decimal);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_guid_values()
        {
            TestSerialization(Guid.NewGuid(), ContextSerializers.Guid);
            TestSerialization(Guid.Empty, ContextSerializers.Guid);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_datetime_values()
        {
            TestSerialization(DateTime.Now, ContextSerializers.DateTime);
            TestSerialization(DateTime.UtcNow, ContextSerializers.DateTime);
            TestSerialization(DateTime.MinValue, ContextSerializers.DateTime);
            TestSerialization(DateTime.MaxValue, ContextSerializers.DateTime);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_datetimeoffset_values()
        {
            TestSerialization(DateTimeOffset.Now, ContextSerializers.DateTimeOffset);
            TestSerialization(DateTimeOffset.UtcNow, ContextSerializers.DateTimeOffset);
            TestSerialization(DateTimeOffset.MinValue, ContextSerializers.DateTimeOffset);
            TestSerialization(DateTimeOffset.MaxValue, ContextSerializers.DateTimeOffset);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_timespan_values()
        {
            TestSerialization(TimeSpan.MinValue, ContextSerializers.TimeSpan);
            TestSerialization(TimeSpan.MaxValue, ContextSerializers.TimeSpan);
            TestSerialization(TimeSpan.Zero, ContextSerializers.TimeSpan);
            TestSerialization(1.Ticks(), ContextSerializers.TimeSpan);
            TestSerialization(1.Milliseconds(), ContextSerializers.TimeSpan);
            TestSerialization(1.Seconds(), ContextSerializers.TimeSpan);
            TestSerialization(1.Days() + 1.Minutes() + 1.Seconds(), ContextSerializers.TimeSpan);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_uri_values()
        {
            TestSerialization(new Uri("https://kontur.ru?a=b", UriKind.Absolute), ContextSerializers.Uri);
            TestSerialization(new Uri("foo/bar?a=b", UriKind.Relative), ContextSerializers.Uri);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_ip_address_values()
        {
            TestSerialization(IPAddress.Loopback, ContextSerializers.IPAddress);
            TestSerialization(IPAddress.IPv6Loopback, ContextSerializers.IPAddress);
            TestSerialization(IPAddress.Parse("1.2.3.4"), ContextSerializers.IPAddress);
        }

        [Test]
        public void Should_correctly_serialize_and_deserialize_enum_values()
        {
            TestSerialization(DayOfWeek.Friday, ContextSerializers.Enum<DayOfWeek>());
            TestSerialization(DayOfWeek.Monday, ContextSerializers.Enum<DayOfWeek>());
        }

        private static void TestSerialization<T>(T value, IContextSerializer<T> serializer)
        {
            serializer.Deserialize(serializer.Serialize(value)).Should().Be(value);
        }
    }
}
