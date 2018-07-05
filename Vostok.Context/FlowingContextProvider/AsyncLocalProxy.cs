using System;
using System.Linq.Expressions;
// ReSharper disable StaticMemberInGenericType

namespace Vostok.Context.FlowingContextProvider
{
    internal class AsyncLocalProxy<T>
    {
        private const string NotSupportedMessage = "AsyncLocal<T> is not supported in current environment.";

        private static readonly bool IsSupported;

        private static readonly Func<object> Constructor;
        private static readonly Func<object, T> Getter;
        private static readonly Action<object, T> Setter;

        static AsyncLocalProxy()
        {
            try
            {
                var type = Type.GetType("System.Threading.AsyncLocal`1");

                if (type == null)
                    return;

                type = type.MakeGenericType(typeof(T));

                var ctor = type.GetConstructor(new Type[] {});
                var valueProperty = type.GetProperty("Value");

                if (ctor == null || valueProperty == null)
                    return;

                Constructor = Expression.Lambda<Func<object>>(Expression.New(ctor)).Compile();

                var instanceParameter = Expression.Parameter(typeof(object), null);
                Getter = Expression.Lambda<Func<object, T>>(
                        Expression.Property(Expression.Convert(instanceParameter, type), valueProperty),
                        instanceParameter)
                    .Compile();

                var valueParameter = Expression.Parameter(typeof(T), null);
                Setter = Expression.Lambda<Action<object, T>>(
                        Expression.Call(Expression.Convert(instanceParameter, type), valueProperty.GetSetMethod(), valueParameter),
                        instanceParameter,
                        valueParameter)
                    .Compile();

                IsSupported = true;
            }
            catch
            {
                // ignored
            }
        }

        private readonly object instance;

        public AsyncLocalProxy()
        {
            if (IsSupported)
                instance = Constructor();
        }

        public T Value
        {
            get
            {
                if (!IsSupported)
                    throw new NotSupportedException(NotSupportedMessage);
                return Getter(instance);
            }
            set
            {
                if (!IsSupported)
                    throw new NotSupportedException(NotSupportedMessage);
                Setter(instance, value);
            }
        }
    }
}