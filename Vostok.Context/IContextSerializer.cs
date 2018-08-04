using JetBrains.Annotations;

namespace Vostok.Context
{
    /// <summary>
    /// Represents a serializer used to stringify and parse context properties and globals values.
    /// </summary>
    [PublicAPI]
    public interface IContextSerializer<T>
    {
        /// <summary>
        /// Serializes given <paramref name="value"/> to its string representation.
        /// </summary>
        string Serialize(T value);

        /// <summary>
        /// Deserializes a value from given non-null string <paramref name="input"/>.
        /// </summary>
        T Deserialize([NotNull] string input);
    }
}