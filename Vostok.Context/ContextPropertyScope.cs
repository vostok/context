using System;

namespace Vostok.Context
{
    internal class ContextPropertyScope : IDisposable
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
}