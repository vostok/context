namespace Vostok.Context.FlowingContextProvider
{
    public class AsyncLocalContextProvider<T> : IFlowingContextProvider<T>
    {
        public static readonly AsyncLocalContextProvider<T> Instance = new AsyncLocalContextProvider<T>();

        private static readonly AsyncLocalProxy<T> Storage = new AsyncLocalProxy<T>();

        public T Get() => Storage.Value;

        public void Set(T value) => Storage.Value = value;
    }
}