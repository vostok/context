using System;
using System.Net;
using JetBrains.Annotations;
using Vostok.Context.Serializers;

namespace Vostok.Context
{
    /// <summary>
    /// <para>Provides serializers for some of the simple built-in types:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="String"/></description></item>
    ///     <item><description><see cref="Char"/></description></item>
    ///     <item><description><see cref="Bool"/></description></item>
    ///     <item><description><see cref="Guid"/></description></item>
    /// 
    ///     <item><description><see cref="Byte"/></description></item>
    ///     <item><description><see cref="Short"/></description></item>
    ///     <item><description><see cref="Int"/></description></item>
    ///     <item><description><see cref="Long"/></description></item>
    /// 
    ///     <item><description><see cref="SignedByte"/></description></item>
    ///     <item><description><see cref="UnsignedShort"/></description></item>
    ///     <item><description><see cref="UnsignedInt"/></description></item>
    ///     <item><description><see cref="UnsignedLong"/></description></item>
    /// 
    ///     <item><description><see cref="Float"/></description></item>
    ///     <item><description><see cref="Double"/></description></item>
    ///     <item><description><see cref="Decimal"/></description></item>
    /// 
    ///     <item><description><see cref="DateTime"/></description></item>
    ///     <item><description><see cref="DateTimeOffset"/></description></item>
    ///     <item><description><see cref="TimeSpan"/></description></item>
    /// 
    ///     <item><description><see cref="Uri"/></description></item>
    ///     <item><description><see cref="IPAddress"/></description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    public static class ContextSerializers
    {
        public static readonly IContextSerializer<string> String = new StringSerializer();
        public static readonly IContextSerializer<char> Char = new CharSerializer();
        public static readonly IContextSerializer<bool> Bool = new BoolSerializer();
        public static readonly IContextSerializer<Guid> Guid = new GuidSerializer();

        public static readonly IContextSerializer<byte> Byte = new ByteSerializer();
        public static readonly IContextSerializer<short> Short = new ShortSerializer();
        public static readonly IContextSerializer<int> Int = new IntSerializer();
        public static readonly IContextSerializer<long> Long = new LongSerializer();

        public static readonly IContextSerializer<sbyte> SignedByte = new SignedByteSerializer();
        public static readonly IContextSerializer<ushort> UnsignedShort = new UnsignedShortSerializer();
        public static readonly IContextSerializer<uint> UnsignedInt = new UnsignedIntSerializer();
        public static readonly IContextSerializer<ulong> UnsignedLong = new UnsignedLongSerializer();

        public static readonly IContextSerializer<float> Float = new FloatSerializer();
        public static readonly IContextSerializer<double> Double = new DoubleSerializer();
        public static readonly IContextSerializer<decimal> Decimal = new DecimalSerializer();

        public static readonly IContextSerializer<DateTime> DateTime = new DateTimeSerializer();
        public static readonly IContextSerializer<DateTimeOffset> DateTimeOffset = new DateTimeOffsetSerializer();
        public static readonly IContextSerializer<TimeSpan> TimeSpan = new TimeSpanSerializer();

        public static readonly IContextSerializer<Uri> Uri = new UriSerializer();
        public static readonly IContextSerializer<IPAddress> IPAddress = new IPAddressSerializer();

        public static IContextSerializer<T> Enum<T>() => new EnumSerializer<T>();
    }
}
