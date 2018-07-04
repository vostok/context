using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Binary;

namespace Vostok.Context
{
    /// <summary>
    /// Delegate for custom serializer
    /// </summary>
    /// <typeparam name="T">Type to serialize</typeparam>
    /// <param name="value">Serializable value</param>
    /// <param name="writer">Binary writer from Vostok.Commons</param>
    public delegate void Serializer<in T>(T value, IBinaryWriter writer);

    /// <summary>
    /// Delegate for custom deserializer
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="reader">Binary reader from Vostok.Commons</param>
    public delegate T Deserializer<out T>(IBinaryReader reader);

    /// <summary>
    /// Context worker
    /// </summary>
    public static class FlowingContext
    {
        private interface ISerializer
        {
            void Serialize(object value, IBinaryWriter writer);
        }

        private interface IDeserializer
        {
            object Deserialize(IBinaryReader reader);
        }

        private static readonly IDictionary<string, ISerializer> Serializers = new Dictionary<string, ISerializer>();
        private static readonly IDictionary<string, IDeserializer> Deserializers = new Dictionary<string, IDeserializer>();
        private static IDictionary<string, ObjectInfo> dictContext = new Dictionary<string, ObjectInfo>();
        private static IDictionary<string, ISerializer> defaultSerializers;
        private static IDictionary<string, IDeserializer> defaultDeserializers;
        private static bool overwriteValues;

        /// <summary>
        /// Add value to context
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Set<T>(string key, T value)
        {
            if (!dictContext.ContainsKey(key))
                dictContext.Add(key, new ObjectInfo {Type = typeof(T).FullName, Object = value});
            else if (overwriteValues)
                dictContext[key] = new ObjectInfo {Type = typeof(T).FullName, Object = value};
            else
                throw new ArgumentException($"Key \"{key}\" already exists");
        }

        /// <summary>
        /// Get value from context
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static T Get<T>(string key) => (T)Get(key);

        public static object Get(string key) => dictContext[key].Object;

        /// <summary>
        /// Serializer from context
        /// </summary>
        /// <returns>Base64 string</returns>
        public static string Serialize()
        {
            var writer = new BinaryBufferWriter(10);
            if (defaultSerializers == null)
                defaultSerializers = new Dictionary<string, ISerializer>
                {
                    {typeof(int).FullName, new SerializerInfo<int>((value, bw) => bw.Write(value))},
                    {typeof(long).FullName, new SerializerInfo<long>((value, bw) => bw.Write(value))},
                    {typeof(short).FullName, new SerializerInfo<short>((value, bw) => bw.Write(value))},
                    {typeof(uint).FullName, new SerializerInfo<uint>((value, bw) => bw.Write(value))},
                    {typeof(ulong).FullName, new SerializerInfo<ulong>((value, bw) => bw.Write(value))},
                    {typeof(ushort).FullName, new SerializerInfo<ushort>((value, bw) => bw.Write(value))},
                    {typeof(byte).FullName, new SerializerInfo<byte>((value, bw) => bw.Write(value))},
                    {typeof(bool).FullName, new SerializerInfo<bool>((value, bw) => bw.Write(value))},
                    {typeof(float).FullName, new SerializerInfo<float>((value, bw) => bw.Write(value))},
                    {typeof(double).FullName, new SerializerInfo<double>((value, bw) => bw.Write(value))},
                    {typeof(Guid).FullName, new SerializerInfo<Guid>((value, bw) => bw.Write(value))},
                    {typeof(string).FullName, new SerializerInfo<string>((value, bw) => bw.Write(value))},
                    {typeof(byte[]).FullName, new SerializerInfo<byte[]>((value, bw) => bw.Write(value))},
                };

            writer.WriteDictionary(
                dictContext,
                (bw, key) => bw.Write(key),
                (bw, info) =>
                {
                    bw.Write(info.Type.ToString());
                    if (Serializers.ContainsKey(info.Type))
                        Serializers[info.Type].Serialize(info.Object, bw);
                    else if (defaultSerializers.ContainsKey(info.Type))
                        defaultSerializers[info.Type].Serialize(info.Object, writer);
                    else
                        throw new FormatException($"{nameof(FlowingContext)}: serializer for type \"{info.Type}\" does not exists");
                });

            return Convert.ToBase64String(writer.FilledSegment.ToArray());
        }

        /// <summary>
        /// Deserializer to context
        /// </summary>
        /// <param name="context">Base64 string</param>
        public static void Deserialize(string context)
        {
            if (defaultDeserializers == null)
                defaultDeserializers = new Dictionary<string, IDeserializer>
                {
                    {typeof(int).FullName, new DeserializerInfo<int>(br => br.ReadInt32())},
                    {typeof(long).FullName, new DeserializerInfo<long>(br => br.ReadInt64())},
                    {typeof(short).FullName, new DeserializerInfo<short>(br => br.ReadInt16())},
                    {typeof(uint).FullName, new DeserializerInfo<uint>(br => br.ReadUInt32())},
                    {typeof(ulong).FullName, new DeserializerInfo<ulong>(br => br.ReadUInt64())},
                    {typeof(ushort).FullName, new DeserializerInfo<ushort>(br => br.ReadUInt16())},
                    {typeof(byte).FullName, new DeserializerInfo<byte>(br => br.ReadByte())},
                    {typeof(bool).FullName, new DeserializerInfo<bool>(br => br.ReadBool())},
                    {typeof(float).FullName, new DeserializerInfo<float>(br => br.ReadFloat())},
                    {typeof(double).FullName, new DeserializerInfo<double>(br => br.ReadDouble())},
                    {typeof(Guid).FullName, new DeserializerInfo<Guid>(br => br.ReadGuid())},
                    {typeof(string).FullName, new DeserializerInfo<string>(br => br.ReadString())},
                    {typeof(byte[]).FullName, new DeserializerInfo<byte[]>(br => br.ReadByteArray())},
                };

            var data = Convert.FromBase64String(context);
            var reader = new BinaryBufferReader(data);

            dictContext = reader.ReadDictionary(
                br => br.ReadString(),
                br =>
                {
                    var info = new ObjectInfo {Type = br.ReadString()};
                    if (Deserializers.ContainsKey(info.Type))
                        info.Object = Deserializers[info.Type].Deserialize(br);
                    else if (defaultDeserializers.ContainsKey(info.Type))
                        info.Object = defaultDeserializers[info.Type].Deserialize(br);
                    else
                        throw new FormatException($"{nameof(FlowingContext)}: deserializer for type \"{info.Type}\" does not exists");
                    return info;
                });
        }

        /// <summary>
        /// Set custom serializer. Redefines default serializers of the same type
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="serializer">Instance of Serializer delegate</param>
        public static void SetSerializer<T>(Serializer<T> serializer) =>
            Serializers.Add(typeof(T).FullName ?? typeof(T).Name, new SerializerInfo<T>(serializer));

        /// <summary>
        /// Set custom deserializer. Redefines default deserializer of the same type
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="deserializer">Instance of Deserializer delegate</param>
        public static void SetDeserializer<T>(Deserializer<T> deserializer) =>
            Deserializers.Add(typeof(T).FullName ?? typeof(T).Name, new DeserializerInfo<T>(deserializer));

        /// <summary>
        /// Sets behavior in case if key exists in context
        /// </summary>
        /// <param name="overwrite">Overwrite key (true) or throw exception (false)</param>
        public static void SetOverwriteMode(bool overwrite) => overwriteValues = overwrite;

        private class ObjectInfo
        {
            public object Object { get; set; }
            public string Type { get; set; }
        }

        private class SerializerInfo<T> : ISerializer
        {
            private readonly Serializer<T> serializeMethod;

            public SerializerInfo(Serializer<T> serializeMethod) =>
                this.serializeMethod = serializeMethod;

            public void Serialize(object value, IBinaryWriter writer) =>
                serializeMethod((T)value, writer);
        }

        private class DeserializerInfo<T> : IDeserializer
        {
            private readonly Deserializer<T> deserializeMethod;

            public DeserializerInfo(Deserializer<T> deserializeMethod) =>
                this.deserializeMethod = deserializeMethod;

            public object Deserialize(IBinaryReader reader) => deserializeMethod(reader);
        }
    }
}