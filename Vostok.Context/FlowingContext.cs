using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Context
{
    /// <summary>
    /// <para>Allows to store and retrieve arbitrary objects from ambient context.</para>
    /// <para>Ambient context propagates by itself into synchronous and asynchronous call chains by the means of <see cref="ExecutionContext"/>.</para>
    /// <para></para>
    /// <para>See <see cref="Globals"/> and <see cref="IContextGlobals"/> for more info on typed global properties.</para>
    /// <para>See <see cref="Properties"/> and <see cref="IContextProperties"/> for more info on named properties.</para>
    /// <para>See <see cref="Configuration"/> and <see cref="IContextConfiguration"/> for more info on configuration options.</para>
    /// </summary>
    [PublicAPI]
    public static class FlowingContext
    {
        static FlowingContext()
        {
            Globals = new ContextGlobals();
            Properties = new ContextProperties();
            Configuration = new ContextConfiguration();
        }

        [NotNull]
        public static IContextGlobals Globals { get; }

        [NotNull]
        public static IContextProperties Properties { get; }

        [NotNull]
        public static IContextConfiguration Configuration { get; }

        [CanBeNull]
        public static string SerializeDistributedGlobals
            => FlowingContextSerializer.SerializeGlobals(Globals, InternalConfiguration);

        [CanBeNull]
        public static string SerializeDistributedProperties
            => FlowingContextSerializer.SerializeProperties(Properties, InternalConfiguration);

        public static void RestoreDistributedGlobals([NotNull] string serialized)
            => FlowingContextRestorer.RestoreGlobals(serialized, Globals, InternalConfiguration);

        public static void RestoreDistributedProperties([NotNull] string serialized)
            => FlowingContextRestorer.RestoreProperties(serialized, Properties, InternalConfiguration);

        private static ContextConfiguration InternalConfiguration 
            => (ContextConfiguration) Configuration;
    }
}
