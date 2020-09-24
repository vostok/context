using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Context
{
    [PublicAPI]
    public static class IContextPropertiesExtensions
    {
        public static TValue Get<TValue>([NotNull] this IContextProperties properties, [NotNull] string name, [CanBeNull] TValue defaultValue = default)
        {
            if (!properties.Current.TryGetValue(name, out var value))
                return defaultValue;

            if (value is TValue typedValue)
                return typedValue;

            return defaultValue;
        }

        public static TValue Get<TValue>([NotNull] this IContextProperties properties, [NotNull] string name, [NotNull] Func<TValue> getDefaultValue)
        {
            if (!properties.Current.TryGetValue(name, out var value))
                return getDefaultValue();

            if (value is TValue typedValue)
                return typedValue;

            return getDefaultValue();
        }

        public static IDisposable Use([NotNull] this IContextProperties properties, [NotNull] string name, [CanBeNull] object value)
        {
            var oldValueExisted = properties.Current.TryGetValue(name, out var oldValue);

            properties.Set(name, value);

            return new ContextPropertyScope(properties, name, oldValue, oldValueExisted);
        }

        public static IDisposable Use([NotNull] this IContextProperties properties, [NotNull] IReadOnlyDictionary<string, object> values)
        {
            var data = new (string, object, bool)[values.Count];
            var dataIndex = data.Length - 1;

            foreach (var pair in values)
            {
                var oldValueExisted = properties.Current.TryGetValue(pair.Key, out var oldValue);

                data[dataIndex--] = (pair.Key, oldValue, oldValueExisted);

                properties.Set(pair.Key, pair.Value);
            }

            return new ContextPropertiesScope(properties, data);
        }

        private class ContextPropertyScope : IDisposable
        {
            private readonly IContextProperties properties;
            private readonly string key;
            private readonly object oldValue;
            private readonly bool oldValueExisted;

            public ContextPropertyScope(IContextProperties properties, string key, object oldValue, bool oldValueExisted)
            {
                this.properties = properties;
                this.key = key;
                this.oldValue = oldValue;
                this.oldValueExisted = oldValueExisted;
            }

            public void Dispose()
            {
                if (oldValueExisted)
                {
                    properties.Set(key, oldValue);
                }
                else
                {
                    properties.Remove(key);
                }
            }
        }

        private class ContextPropertiesScope : IDisposable
        {
            private readonly IContextProperties properties;
            private readonly (string, object, bool)[] data;

            public ContextPropertiesScope(IContextProperties properties, (string, object, bool)[] data)
            {
                this.properties = properties;
                this.data = data;
            }

            public void Dispose()
            {
                foreach (var (name, oldValue, oldValueExisted) in data)
                {
                    if (oldValueExisted)
                    {
                        properties.Set(name, oldValue);
                    }
                    else
                    {
                        properties.Remove(name);
                    }
                }
            }
        }
    }
}