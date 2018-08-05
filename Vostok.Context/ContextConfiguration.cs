using System;
using System.Collections.Concurrent;
using Vostok.Context.Helpers;

namespace Vostok.Context
{
    internal class ContextConfiguration : IContextConfiguration
    {
        public IContextErrorListener ErrorListener { get; set; }

        public ConcurrentDictionary<string, (Type, IContextSerializer)> DistributedProperties { get; }
            = new ConcurrentDictionary<string, (Type, IContextSerializer)>(StringComparer.Ordinal);

        public ConcurrentDictionary<string, (Type, IContextSerializer)> DistributedGlobals { get; }
            = new ConcurrentDictionary<string, (Type, IContextSerializer)>(StringComparer.Ordinal);

        public void RegisterDistributedProperty<T>(string name, IContextSerializer<T> serializer)
        {
            DistributedProperties[name] = (typeof(T), new GenericSerializer<T>(serializer));
        }

        public void RegisterDistributedGlobal<T>(string name, IContextSerializer<T> serializer)
        {
            DistributedGlobals[name] = (typeof(T), new GenericSerializer<T>(serializer));
        }
    }
}
