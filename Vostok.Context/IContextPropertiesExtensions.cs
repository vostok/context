using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    // TODO(iloktionov): 1. Tests
    // TODO(iloktionov): 2. Use() for multiple properties

    [PublicAPI]
    public static class IContextPropertiesExtensions
    {
        public static TValue Get<TValue>([NotNull] this IContextProperties properties, [NotNull] string key, TValue defaultValue = default)
        {
            if (!properties.Current.TryGetValue(key, out var value))
                return defaultValue;

            if (value is TValue typedValue)
                return typedValue;

            return defaultValue;
        }

        public static IDisposable Use([NotNull] this IContextProperties properties, [NotNull] string key, object value)
        {
            var oldValueExisted = properties.Current.TryGetValue(key, out var oldValue);

            properties.Set(key, value);

            return new ContextPropertyScope(properties, key, oldValue, oldValueExisted);
        }
    }
}