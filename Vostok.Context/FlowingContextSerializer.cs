using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Context.Helpers;

namespace Vostok.Context
{
    internal static class FlowingContextSerializer
    {
        private const int DefaultStringWriterCapacity = 64;

        private static readonly UnboundedObjectPool<BinaryStringWriter> StringWritersPool
            = new UnboundedObjectPool<BinaryStringWriter>(() => new BinaryStringWriter(DefaultStringWriterCapacity));

        [CanBeNull]
        public static string SerializeGlobals(
            [NotNull] ContextGlobals globals,
            [NotNull] ContextConfiguration configuration)
        {
            return SerializeInternal(EnumerateGlobals(globals, configuration), configuration.ErrorCallback);
        }

        [CanBeNull]
        public static string SerializeProperties(
            [NotNull] ContextProperties properties,
            [NotNull] ContextConfiguration configuration)
        {
            return SerializeInternal(EnumerateProperties(properties, configuration), configuration.ErrorCallback);
        }

        [CanBeNull]
        public static string WriteProperties([NotNull] IEnumerable<(string, string)> input, [CanBeNull] Action<string, Exception> errorCallback)
        {
            return SerializeInternal(input.Select(i => (i.Item1, (object)i.Item2, (IContextSerializer)RawStringSerializer.Instance)), errorCallback);
        }

        private static IEnumerable<(string, object, IContextSerializer)> EnumerateGlobals(
            ContextGlobals globals,
            ContextConfiguration configuration)
        {
            foreach (var pair in configuration.DistributedGlobals)
            {
                var currentValue = globals.Get(pair.Value.type);
                if (currentValue == null)
                    continue;

                yield return (pair.Key, currentValue, pair.Value.serializer);
            }
        }

        private static IEnumerable<(string, object, IContextSerializer)> EnumerateProperties(
            ContextProperties properties,
            ContextConfiguration configuration)
        {
            var snapshot = properties.Current;

            foreach (var pair in configuration.DistributedProperties)
            {
                if (!snapshot.TryGetValue(pair.Key, out var currentValue))
                    continue;

                if (currentValue == null)
                    continue;

                yield return (pair.Key, currentValue, pair.Value);
            }
        }

        [CanBeNull]
        private static string SerializeInternal(
            [NotNull] IEnumerable<(string, object, IContextSerializer)> properties,
            [CanBeNull] Action<string, Exception> errorCallback)
        {
            using (StringWritersPool.Acquire(out var writer))
            {
                writer.Clear();

                foreach (var (name, value, serializer) in properties)
                {
                    var serializedValue = SerializeValue(name, value, serializer, errorCallback);
                    if (serializedValue == null)
                        continue;

                    writer.Write(name);
                    writer.Write(serializedValue);
                }

                return writer.IsEmpty ? null : writer.ToBase64String();
            }
        }

        [CanBeNull]
        private static string SerializeValue(
            [NotNull] string name,
            [NotNull] object value,
            [NotNull] IContextSerializer serializer,
            [CanBeNull] Action<string, Exception> errorCallback)
        {
            try
            {
                return serializer.Serialize(value);
            }
            catch (Exception error)
            {
                errorCallback?.Invoke($"Failed to serialize property '{name}' of type '{value.GetType().Name}'.", error);

                return null;
            }
        }
    }
}