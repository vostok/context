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

        public bool Set(string name, object value, bool allowOverwrite = true)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var propertiesBefore = Properties;
            var propertiesAfter = propertiesBefore.Set(name, value, allowOverwrite);

            if (ReferenceEquals(propertiesBefore, propertiesAfter))
                return false;

            Properties = propertiesAfter;

            return true;
        }

        public void Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var propertiesBefore = Properties;
            var propertiesAfter = propertiesBefore.Remove(name);

            if (ReferenceEquals(propertiesBefore, propertiesAfter))
                return;

            Properties = propertiesAfter;
        }
    }
}
