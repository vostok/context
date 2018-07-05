namespace Vostok.Context.FlowingContextProvider
{
    public interface IFlowingContextProvider<T>
    {
        T Get();
        void Set(T value);
    }
}