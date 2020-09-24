using JetBrains.Annotations;

namespace Vostok.Context
{
    /// <summary>
    /// <para>Represents mutable type-based ambient context properties.</para>
    /// <para>These properties are global in a sense that they can only hold one value of each type.</para>
    /// <para>See <see cref="Get{T}"/> and <see cref="Set{T}"/> for more details.</para>
    /// </summary>
    [PublicAPI]
    public interface IContextGlobals
    {
        /// <summary>
        /// <para>Returns current global contextual value for type <typeparamref name="T"/>.</para>
        /// <para>If no value is set, returns <typeparamref name="T"/>'s default value.</para>
        /// </summary>
        [CanBeNull]
        T Get<T>();

        /// <summary>
        /// <para>Sets the global property of type <typeparamref name="T"/> to given <paramref name="value"/>.</para>
        /// <para>There is a "copy-on-write" mechanism for certain threading scenarios: any <see cref="Set{T}"/> calls made after entering an async method, yielding control with <c>await</c> or spinning up a new task/thread do not affect upstream context.</para>
        /// </summary>
        void Set<T>([CanBeNull] T value);
    }
}