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

        /// <summary>
        /// <para>Serializes all context globals registered with <see cref="IContextConfiguration.RegisterDistributedGlobal{T}"/> in configuration and having non-null-values.</para>
        /// <para>Returned string is encoded in Base64 and can be transmitted in HTTP headers as-is.</para>
        /// <para>Returned string is meant to be used to <see cref="RestoreDistributedGlobals"/> on a different process/machine later.</para>
        /// <para>May return null if there were no non-null global values to serialize.</para>
        /// </summary>
        [CanBeNull]
        public static string SerializeDistributedGlobals
            => FlowingContextSerializer.SerializeGlobals(globals, configuration);

        /// <summary>
        /// <para>Serializes all context properties registered with <see cref="IContextConfiguration.RegisterDistributedProperty{T}"/> in configuration and having non-null-values.</para>
        /// <para>Returned string is encoded in Base64 and can be transmitted in HTTP headers as-is.</para>
        /// <para>Returned string is meant to be used to <see cref="RestoreDistributedProperties"/> on a different process/machine later.</para>
        /// <para>May return null if there were no non-null property values to serialize.</para>
        /// </summary>
        [CanBeNull]
        public static string SerializeDistributedProperties
            => FlowingContextSerializer.SerializeProperties(properties, configuration);

        /// <summary>
        /// <para>Deserializes all globals stored in given <paramref name="serialized"/> string, then restores those of them registered with <see cref="IContextConfiguration.RegisterDistributedGlobal{T}"/> in configuration to context.</para>
        /// <para>Input string is expected to be a result of <see cref="SerializeDistributedGlobals"/> call elsewhere.</para>
        /// </summary>
        public static void RestoreDistributedGlobals([NotNull] string serialized)
            => FlowingContextRestorer.RestoreGlobals(serialized, globals, configuration);

        /// <summary>
        /// <para>Deserializes all properties stored in given <paramref name="serialized"/> string, then restores those of them registered with <see cref="IContextConfiguration.RegisterDistributedProperty{T}"/> in configuration to context.</para>
        /// <para>Input string is expected to be a result of <see cref="SerializeDistributedProperties"/> call elsewhere.</para>
        /// </summary>
        public static void RestoreDistributedProperties([NotNull] string serialized)
            => FlowingContextRestorer.RestoreProperties(serialized, properties, configuration);
    }
}
