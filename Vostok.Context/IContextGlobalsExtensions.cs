using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    [PublicAPI]
    public static class IContextGlobalsExtensions
    {
        public static void SetValueStorage<T>([NotNull] this IContextGlobals globals, Func<T> getter, Action<T> setter)
        {
            if (globals is not ContextGlobals contextGlobals)
                throw new ArgumentException($"{globals.GetType()} isn't {nameof(ContextGlobals)}");
            contextGlobals.SetValueStorage(getter, setter);
        }

        public static IDisposable Use<T>([NotNull] this IContextGlobals globals, [CanBeNull] T value)
        {
            return Use(globals, value, out _);
        }

        public static IDisposable Use<T>([NotNull] this IContextGlobals globals, [CanBeNull] T value, [CanBeNull] out T oldValue)
        {
            oldValue = globals.Get<T>();

            globals.Set(value);

            return new ContextGlobalScope<T>(globals, oldValue);
        }

        private class ContextGlobalScope<T> : IDisposable
        {
            private readonly IContextGlobals globals;
            private readonly T oldValue;

            public ContextGlobalScope(IContextGlobals globals, T oldValue)
            {
                this.globals = globals;
                this.oldValue = oldValue;
            }

            public void Dispose()
            {
                globals.Set(oldValue);
            }
        }
    }
}