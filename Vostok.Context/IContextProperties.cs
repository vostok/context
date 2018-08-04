using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Context
{
    /// <summary>
    /// <para>Represents mutable ambient context properties.</para>
    /// <para>See <see cref="Current"/>, <see cref="Set"/> and <see cref="Remove"/> for details.</para>
    /// </summary>
    [PublicAPI]
    public interface IContextProperties
    {
        /// <summary>
        /// <para>Returns a current snapshot of all context properties.</para>
        /// <para>Once obtained, the snapshot is immutable and is not affected by any subsequent <see cref="Set"/> and <see cref="Remove"/> calls.</para>
        /// <para>Additionally there is a "copy-on-write" mechanism for certain threading scenarios: any changes made to properties after entering an async method, yielding control with <c>await</c> or spinning up a new task/thread do not affect upstream context, even if no snapshot was obtained beforehand.</para>
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<string, object> Current { get; }

        /// <summary>
        /// <para>Sets given <paramref name="value"/> for a property with given <paramref name="key"/>.</para>
        /// <para>If a property with such <paramref name="key"/> does not exist, it gets created.</para>
        /// <para>If a property with such <paramref name="key"/> and a different value exists, it gets updated only if <paramref name="allowOverwrite"/> is <c>true</c>.</para>
        /// </summary>
        /// <returns><c>true</c> if properties were changed, <c>false</c> otherwise (a conflict occured or the property already had same value).</returns>
        bool Set([NotNull] string key, [CanBeNull] object value, bool allowOverwrite = true);

        /// <summary>
        /// Removes the property with given <paramref name="key"/>. Has no effect if no such property exists.
        /// </summary>
        void Remove([NotNull] string key);
    }
}
