using System.Threading;

namespace Vostok.Context
{
    internal class ContextGlobals : IContextGlobals
    {
        public T Get<T>()
        {
            return Container<T>.AsyncLocal.Value;
        }

        public void Set<T>(T value)
        {
            Container<T>.AsyncLocal.Value = value;
        }

        private static class Container<T>
        {
            public static readonly AsyncLocal<T> AsyncLocal = new AsyncLocal<T>();
        }
    }
}