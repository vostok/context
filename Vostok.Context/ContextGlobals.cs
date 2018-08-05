using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute

namespace Vostok.Context
{
    internal class ContextGlobals : IContextGlobals
    {
        private readonly ConcurrentDictionary<Type, Func<object>> Getters = new ConcurrentDictionary<Type, Func<object>>();
        private readonly ConcurrentDictionary<Type, Action<object>> Setters = new ConcurrentDictionary<Type, Action<object>>();

        public T Get<T>()
        {
            return Container<T>.AsyncLocal.Value;
        }

        public void Set<T>(T value)
        {
            Container<T>.AsyncLocal.Value = value;
        }

        public object Get(Type type)
        {
            return Getters.GetOrAdd(type, CompileGetter)();
        }

        public void Set(Type type, object value)
        {
            Setters.GetOrAdd(type, CompileSetter)(value);
        }

        private static Func<object> CompileGetter(Type type)
        {
            var containerType = typeof(Container<>).MakeGenericType(type);
            var asyncLocalField = containerType.GetField(nameof(Container<int>.AsyncLocal), BindingFlags.Static | BindingFlags.Public);

            var asyncLocalAccess = Expression.MakeMemberAccess(null, asyncLocalField);
            var valueAccess = Expression.Property(asyncLocalAccess, nameof(AsyncLocal<int>.Value));
            var objectCast = Expression.Convert(valueAccess, typeof(object));

            return Expression.Lambda<Func<object>>(objectCast).Compile();
        }

        private static Action<object> CompileSetter(Type type)
        {
            var containerType = typeof(Container<>).MakeGenericType(type);
            var asyncLocalField = containerType.GetField(nameof(Container<int>.AsyncLocal), BindingFlags.Static | BindingFlags.Public);
            var asyncLocalType = asyncLocalField.FieldType;

            var valueProperty = asyncLocalType.GetProperty(nameof(AsyncLocal<int>.Value), BindingFlags.Instance | BindingFlags.Public);
            var valuePropertySetter = valueProperty.GetSetMethod();

            var parameter = Expression.Parameter(typeof(object));
            var castedParameter = Expression.Convert(parameter, type);

            var asyncLocalAccess = Expression.MakeMemberAccess(null, asyncLocalField);
            var propertySetter = Expression.Call(asyncLocalAccess, valuePropertySetter, castedParameter);

            return Expression.Lambda<Action<object>>(propertySetter, parameter).Compile();
        }

        private static class Container<T>
        {
            public static readonly AsyncLocal<T> AsyncLocal = new AsyncLocal<T>();
        }
    }
}
