using JetBrains.Annotations;

namespace Vostok.Context
{
    [PublicAPI]
    public interface IContextConfiguration
    {
        /// <summary>
        /// Gets/sets a listener that will be notified on each internal error in context management.
        /// </summary>
        [CanBeNull]
        IContextErrorListener ErrorListener { get; set; }

        /// <summary>
        /// <para>Adds a named property of type <typeparamref name="T"/> to a whitelist of distributed properties.</para>
        /// <para>It will be serialized using <paramref name="serializer"/> on every <see cref="FlowingContext.SerializeDistributedProperties"/> call.</para>
        /// <para>It will be deserialized using <paramref name="serializer"/> on every <see cref="FlowingContext.RestoreDistributedProperties"/> call.</para>
        /// </summary>
        void RegisterDistributedProperty<T>([NotNull] string name, [NotNull] IContextSerializer<T> serializer);

        /// <summary>
        /// <para>Adds a global property of type <typeparamref name="T"/> to a whitelist of distributed globals.</para>
        /// <para>It will be serialized using <paramref name="serializer"/> on every <see cref="FlowingContext.SerializeDistributedGlobals"/> call.</para>
        /// <para>It will be deserialized using <paramref name="serializer"/> on every <see cref="FlowingContext.RestoreDistributedGlobals"/> call.</para>
        /// </summary>
        void RegisterDistributedGlobal<T>([NotNull] string name, [NotNull] IContextSerializer<T> serializer);
    }
}
