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

        public bool SetCapacity(int capacity)
        {
            if (container.Value != null)
                return false;

            container.Value = new ImmutableArrayDictionary<string, object>(capacity);
            return true;

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

        public bool SetRange((string name, object value)[] newPairs, bool allowOverwrite = true)
        {
            for (var i = 0; i < newPairs.Length; i++)
                if (newPairs[i].name == null)
                    throw new ArgumentException("Keys of new pairs should not be null", nameof(newPairs));

            var propertiesBefore = Properties;
            var propertiesAfter = propertiesBefore.SetRange(newPairs, allowOverwrite);

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

        public void Clear()
        {
            Properties = ImmutableArrayDictionary<string, object>.Empty;
        }

        private ImmutableArrayDictionary<string, object> Properties
        {
            get => container.Value ?? EmptyProperties;
            set => container.Value = value;
        }
    }
}