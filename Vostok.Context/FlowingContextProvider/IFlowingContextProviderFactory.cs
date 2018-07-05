namespace Vostok.Context.FlowingContextProvider
{
    public interface IFlowingContextProviderFactory
    {
        IFlowingContextProvider<T> Obtain<T>();
    }
}