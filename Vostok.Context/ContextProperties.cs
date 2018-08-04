using System;
using System.Collections.Generic;
using System.Threading;
using Vostok.Commons.Collections;

namespace Vostok.Context
{
    internal class ContextProperties : IContextProperties
    {
        private static readonly ImmutableArrayDictionary<string, object> EmptyProperties
            = new ImmutableArrayDictionary<string, object>(0, StringComparer.Ordinal);

        private readonly AsyncLocal<ImmutableArrayDictionary<string, object>> container;

        public ContextProperties()
        {
            container = new AsyncLocal<ImmutableArrayDictionary<string, object>>();
        }

        public IReadOnlyDictionary<string, object> Current => Properties;

        private ImmutableArrayDictionary<string, object> Properties
        {
            get => container.Value ?? EmptyProperties;
            set => container.Value = value;
        }

        public bool Set(string key, object value, bool allowOverwrite = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var propertiesBefore = Properties;
            var propertiesAfter = propertiesBefore.Set(key, value, allowOverwrite);

            if (ReferenceEquals(propertiesBefore, propertiesAfter))
                return false;

            Properties = propertiesAfter;

            return true;
        }

        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var propertiesBefore = Properties;
            var propertiesAfter = propertiesBefore.Remove(key);

            if (ReferenceEquals(propertiesBefore, propertiesAfter))
                return;

            Properties = propertiesAfter;
        }
    }
}
