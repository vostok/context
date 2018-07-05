namespace Vostok.Context.FlowingContextProvider
{
    public class AsyncLocalContextProviderFactory : IFlowingContextProviderFactory
    {
        public IFlowingContextProvider<T> Obtain<T>() => AsyncLocalContextProvider<T>.Instance;
    }
}