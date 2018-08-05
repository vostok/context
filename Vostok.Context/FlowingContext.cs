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
        private static readonly ContextGlobals globals = new ContextGlobals();
        private static readonly ContextProperties properties = new ContextProperties();
        private static readonly ContextConfiguration configuration = new ContextConfiguration();

        [NotNull]
        public static IContextGlobals Globals => globals;

        [NotNull]
        public static IContextProperties Properties => properties;

        [NotNull]
        public static IContextConfiguration Configuration => configuration;

        [CanBeNull]
        public static string SerializeDistributedGlobals
            => FlowingContextSerializer.SerializeGlobals(globals, configuration);

        [CanBeNull]
        public static string SerializeDistributedProperties
            => FlowingContextSerializer.SerializeProperties(properties, configuration);

        public static void RestoreDistributedGlobals([NotNull] string serialized)
            => FlowingContextRestorer.RestoreGlobals(serialized, globals, configuration);

        public static void RestoreDistributedProperties([NotNull] string serialized)
            => FlowingContextRestorer.RestoreProperties(serialized, properties, configuration);
    }
}
