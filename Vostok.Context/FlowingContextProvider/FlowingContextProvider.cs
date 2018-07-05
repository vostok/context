namespace Vostok.Context.FlowingContextProvider
{
    public static class FlowingContextProvider
    {
        private static IFlowingContextProviderFactory providerFactory;

        static FlowingContextProvider() => providerFactory = new AsyncLocalContextProviderFactory();

        public static void Configure(IFlowingContextProviderFactory factory) => providerFactory = factory;

        public static T Get<T>() => providerFactory.Obtain<T>().Get();

        public static void Set<T>(T value) => providerFactory.Obtain<T>().Set(value);
    }
}