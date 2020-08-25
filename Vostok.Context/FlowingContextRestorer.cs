using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Context.Helpers;

namespace Vostok.Context
{
    internal static class FlowingContextRestorer
    {
        public static void RestoreGlobals(
            [CanBeNull] string input,
            [NotNull] ContextGlobals globals,
            [NotNull] ContextConfiguration configuration)
        {
            foreach (var (name, serializedValue) in ReadProperties(input, configuration.ErrorCallback))
            {
                if (!configuration.DistributedGlobals.TryGetValue(name, out var tuple))
                    continue;

                var value = DeserializeValue(name, serializedValue, tuple.serializer, configuration.ErrorCallback);
                if (value == null)
                    continue;

                globals.Set(tuple.type, value);
            }
        }

        public static void RestoreProperties(
            [CanBeNull] string input,
            [NotNull] ContextProperties properties,
            [NotNull] ContextConfiguration configuration)
        {
            foreach (var (name, serializedValue) in ReadProperties(input, configuration.ErrorCallback))
            {
                if (!configuration.DistributedProperties.TryGetValue(name, out var serializer))
                    continue;

                var value = DeserializeValue(name, serializedValue, serializer, configuration.ErrorCallback);
                if (value == null)
                    continue;

                properties.Set(name, value);
            }
        }

        [NotNull]
        public static IEnumerable<(string, string)> ReadProperties([CanBeNull] string input, [CanBeNull] Action<string, Exception> errorCallback)
        {
            if (input == null)
                return Array.Empty<(string, string)>();

            try
            {
                var properties = new List<(string, string)>();
                var reader = new BinaryStringReader(Convert.FromBase64String(input));

                while (reader.HasDataLeft)
                {
                    var name = reader.Read();
                    var value = reader.Read();

                    properties.Add((name, value));
                }

                return properties;
            }
            catch (Exception error)
            {
                errorCallback?.Invoke($"Failed to read property names and values from input string '{input}'.", error);

                return Array.Empty<(string, string)>();
            }
        }

        [CanBeNull]
        private static object DeserializeValue(
            string name,
            string input,
            IContextSerializer serializer,
            Action<string, Exception> errorCallback)
        {
            try
            {
                return serializer.Deserialize(input);
            }
            catch (Exception error)
            {
                errorCallback?.Invoke($"Failed to deserialize value of property '{name}' from following string: '{input}'.", error);

                return null;
            }
        }
    }
}