using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
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
        private static readonly AsyncLocal<IDictionary<string, object>> Storage = new AsyncLocal<IDictionary<string, object>> {Value = new Dictionary<string, object>()};
        private static IDictionary<Type, ISerializer> defaultSerializers;
        private static IDictionary<Type, IDeserializer> defaultDeserializers;
        private static bool overwriteValues;

        /// <summary>
        /// Add value to context
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Set<T>([NotNull] string key, T value)
        {
            if (Storage.Value == null)
                Storage.Value = new Dictionary<string, object>();

            if (overwriteValues)
                Storage.Value[key] = value;
            else if (!Storage.Value.ContainsKey(key))
                Storage.Value.Add(key, value);
            else
                throw new ArgumentException($"Key \"{key}\" already exists");
        }

        /// <summary>
        /// Get value from context
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        public static T Get<T>([NotNull] string key) => (T)(Get(key) ?? default(T));

        public static object Get([NotNull] string key)
        {
            if (Storage.Value == null)
                Storage.Value = new Dictionary<string, object>();
            return Storage.Value.TryGetValue(key, out var value) ? value : null;
        }

        public static void Remove([NotNull] string key) => Storage.Value?.Remove(key);

        /// <summary>
        /// Serializer from context
        /// </summary>
        /// <returns>Base64 string</returns>
        public static string Serialize()
        {
            var writer = new BinaryBufferWriter(10);
            if (defaultSerializers == null)
                defaultSerializers = new Dictionary<Type, ISerializer>
                {
                    {typeof(int), new SerializerInfo<int>((value, bw) => bw.Write(value))},
                    {typeof(long), new SerializerInfo<long>((value, bw) => bw.Write(value))},
                    {typeof(short), new SerializerInfo<short>((value, bw) => bw.Write(value))},
                    {typeof(uint), new SerializerInfo<uint>((value, bw) => bw.Write(value))},
                    {typeof(ulong), new SerializerInfo<ulong>((value, bw) => bw.Write(value))},
                    {typeof(ushort), new SerializerInfo<ushort>((value, bw) => bw.Write(value))},
                    {typeof(byte), new SerializerInfo<byte>((value, bw) => bw.Write(value))},
                    {typeof(bool), new SerializerInfo<bool>((value, bw) => bw.Write(value))},
                    {typeof(float), new SerializerInfo<float>((value, bw) => bw.Write(value))},
                    {typeof(double), new SerializerInfo<double>((value, bw) => bw.Write(value))},
                    {typeof(Guid), new SerializerInfo<Guid>((value, bw) => bw.Write(value))},
                    {typeof(string), new SerializerInfo<string>((value, bw) => bw.Write(value))},
                    {typeof(byte[]), new SerializerInfo<byte[]>((value, bw) => bw.Write(value))},
                };
            var storage = Storage.Value;

            writer.WriteDictionary(
                storage,
                (bw, key) => bw.Write(key),
                (bw, obj) =>
                {
                    var type = obj.GetType();
                    var typeName = TypeName(type);
                    bw.Write(type.FullName ?? type.Name);
                    if (Serializers.ContainsKey(typeName))
                        Serializers[typeName].Serialize(obj, bw);
                    else if (defaultSerializers.ContainsKey(type))
                        defaultSerializers[type].Serialize(obj, bw);
                    else
                        throw new FormatException($"{nameof(FlowingContext)}: serializer for type \"{type}\" does not exists");
                });

            return Convert.ToBase64String(writer.FilledSegment.ToArray());
        }

        /// <summary>
        /// Deserializer to context
        /// </summary>
        /// <param name="context">Base64 string</param>
        public static void Deserialize([NotNull] string context)
        {
            if (defaultDeserializers == null)
                defaultDeserializers = new Dictionary<Type, IDeserializer>
                {
                    {typeof(int), new DeserializerInfo<int>(br => br.ReadInt32())},
                    {typeof(long), new DeserializerInfo<long>(br => br.ReadInt64())},
                    {typeof(short), new DeserializerInfo<short>(br => br.ReadInt16())},
                    {typeof(uint), new DeserializerInfo<uint>(br => br.ReadUInt32())},
                    {typeof(ulong), new DeserializerInfo<ulong>(br => br.ReadUInt64())},
                    {typeof(ushort), new DeserializerInfo<ushort>(br => br.ReadUInt16())},
                    {typeof(byte), new DeserializerInfo<byte>(br => br.ReadByte())},
                    {typeof(bool), new DeserializerInfo<bool>(br => br.ReadBool())},
                    {typeof(float), new DeserializerInfo<float>(br => br.ReadFloat())},
                    {typeof(double), new DeserializerInfo<double>(br => br.ReadDouble())},
                    {typeof(Guid), new DeserializerInfo<Guid>(br => br.ReadGuid())},
                    {typeof(string), new DeserializerInfo<string>(br => br.ReadString())},
                    {typeof(byte[]), new DeserializerInfo<byte[]>(br => br.ReadByteArray())},
                };

            var data = Convert.FromBase64String(context);
            var reader = new BinaryBufferReader(data);

            Storage.Value = reader.ReadDictionary(
                br => br.ReadString(),
                br =>
                {
                    var typeName = br.ReadString();
                    var type = Type.GetType(typeName);
                    object obj;
                    if (Deserializers.ContainsKey(typeName))
                        obj = Deserializers[typeName].Deserialize(br);
                    else if (defaultDeserializers.ContainsKey(type))
                        obj = defaultDeserializers[type].Deserialize(br);
                    else
                        throw new FormatException($"{nameof(FlowingContext)}: deserializer for type \"{type}\" does not exists");
                    return obj;
                });
        }

        /// <summary>
        /// Set custom serializer. Redefines default serializers of the same type
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="serializer">Instance of Serializer delegate</param>
        public static void SetSerializer<T>([NotNull] Serializer<T> serializer) =>
            Serializers.Add(TypeName<T>(), new SerializerInfo<T>(serializer));

        /// <summary>
        /// Set custom deserializer. Redefines default deserializer of the same type
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="deserializer">Instance of Deserializer delegate</param>
        public static void SetDeserializer<T>([NotNull] Deserializer<T> deserializer) =>
            Deserializers.Add(TypeName<T>(), new DeserializerInfo<T>(deserializer));

        /// <summary>
        /// Sets behavior in case if key exists in context
        /// </summary>
        /// <param name="overwrite">Overwrite key (true) or throw exception (false)</param>
        public static void SetOverwriteMode(bool overwrite) => overwriteValues = overwrite;

        private static string TypeName<T>() => TypeName(typeof(T));
        private static string TypeName(Type type) => type.FullName ?? type.Name;

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