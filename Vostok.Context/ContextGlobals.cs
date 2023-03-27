using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Context
{
    internal class ContextGlobals : IContextGlobals
    {
        private readonly ConcurrentDictionary<Type, Func<object>> getters = new ConcurrentDictionary<Type, Func<object>>();
        private readonly ConcurrentDictionary<Type, Action<object>> setters = new ConcurrentDictionary<Type, Action<object>>();

        public void SetValueStorage<T>(Func<T> getter, Action<T> setter) => Container<T>.SetValueProvider(getter, setter);

        public T Get<T>() => Container<T>.Value;

        public void Set<T>(T value) => Container<T>.Value = value;

        public object Get(Type type) => getters.GetOrAdd(type, CompileGetter)();

        public void Set(Type type, object value) => setters.GetOrAdd(type, CompileSetter)(value);

        private static Func<object> CompileGetter(Type type)
        {
            var containerType = typeof(Container<>).MakeGenericType(type);

            var valueProperty = containerType.GetProperty(nameof(Container<int>.Value), BindingFlags.Static | BindingFlags.Public);
            var valuePropertyGetter = valueProperty.GetGetMethod();

            var propertyGetter = Expression.Call(valuePropertyGetter);
            var objectCast = Expression.Convert(propertyGetter, typeof(object));

            return Expression.Lambda<Func<object>>(objectCast).Compile();
        }

        private static Action<object> CompileSetter(Type type)
        {
            var containerType = typeof(Container<>).MakeGenericType(type);

            var valueProperty = containerType.GetProperty(nameof(Container<int>.Value), BindingFlags.Static | BindingFlags.Public);
            var valuePropertySetter = valueProperty.GetSetMethod();

            var parameter = Expression.Parameter(typeof(object));
            var castedParameter = Expression.Convert(parameter, type);
            var propertySetter = Expression.Call(valuePropertySetter, castedParameter);

            return Expression.Lambda<Action<object>>(propertySetter, parameter).Compile();
        }

        private static class Container<T>
        {
            private static readonly AsyncLocal<T> AsyncLocal = new AsyncLocal<T>();
            private static Func<T> getter;
            private static Action<T> setter;

            public static T Value
            {
                get =>
                    getter == null ? AsyncLocal.Value : getter();
                set
                {
                    if (setter == null)
                        AsyncLocal.Value = value;
                    else
                        setter(value);
                }
            }

            [SuppressMessage("ReSharper", "ParameterHidesMember")]
            public static void SetValueProvider(Func<T> getter, Action<T> setter)
            {
                Container<T>.getter = getter;
                Container<T>.setter = setter;
            }
        }
    }
}