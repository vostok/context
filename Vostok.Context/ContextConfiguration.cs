using System;
using System.Collections.Concurrent;
using Vostok.Context.Helpers;

namespace Vostok.Context
{
    internal class ContextConfiguration : IContextConfiguration
    {
        public Action<string, Exception> ErrorCallback { get; set; }

        public ConcurrentDictionary<string, IContextSerializer> DistributedProperties { get; }
            = new ConcurrentDictionary<string, IContextSerializer>(StringComparer.Ordinal);

        public ConcurrentDictionary<string, (Type type, IContextSerializer serializer)> DistributedGlobals { get; }
            = new ConcurrentDictionary<string, (Type type, IContextSerializer serializer)>(StringComparer.Ordinal);

        public void RegisterDistributedProperty<T>(string name, IContextSerializer<T> serializer)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            DistributedProperties[name] = new GenericSerializer<T>(serializer);
        }

        public void RegisterDistributedGlobal<T>(string name, IContextSerializer<T> serializer)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            DistributedGlobals[name] = (typeof(T), new GenericSerializer<T>(serializer));
        }

        public void ClearDistributedProperties()
        {
            DistributedProperties.Clear();
        }

        public void ClearDistributedGlobals()
        {
            DistributedGlobals.Clear();
        }
    }
}
