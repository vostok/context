using System;
using JetBrains.Annotations;

namespace Vostok.Context
{
    [PublicAPI]
    public interface IContextConfiguration
    {
        /// <summary>
        /// Gets/sets a callback that will be invoked on each internal error in context management.
        /// </summary>
        [CanBeNull]
        Action<string, Exception> ErrorCallback { get; set; }

        /// <summary>
        /// <para>Adds a named property of type <typeparamref name="T"/> to a whitelist of distributed properties.</para>
        /// <para>It will be serialized using <paramref name="serializer"/> on every <see cref="FlowingContext.SerializeDistributedProperties"/> call.</para>
        /// <para>It will be deserialized using <paramref name="serializer"/> on every <see cref="FlowingContext.RestoreDistributedProperties"/> call.</para>
        /// <para>See <see cref="ContextSerializers"/> for built-in serializers of primitive and simple types.</para>
        /// </summary>
        void RegisterDistributedProperty<T>([NotNull] string name, [NotNull] IContextSerializer<T> serializer);

        /// <summary>
        /// <para>Adds a global property of type <typeparamref name="T"/> to a whitelist of distributed globals.</para>
        /// <para>It will be serialized using <paramref name="serializer"/> on every <see cref="FlowingContext.SerializeDistributedGlobals"/> call.</para>
        /// <para>It will be deserialized using <paramref name="serializer"/> on every <see cref="FlowingContext.RestoreDistributedGlobals"/> call.</para>
        /// <para>See <see cref="ContextSerializers"/> for built-in serializers of primitive and simple types.</para>
        /// </summary>
        void RegisterDistributedGlobal<T>([NotNull] string name, [NotNull] IContextSerializer<T> serializer);

        /// <summary>
        /// Removes all registered distributed properties from the whitelist.
        /// </summary>
        void ClearDistributedProperties();

        /// <summary>
        /// Removes all registered distributed globals from the whitelist.
        /// </summary>
        void ClearDistributedGlobals();
    }
}