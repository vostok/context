using JetBrains.Annotations;

namespace Vostok.Context
{
    internal interface IContextSerializer
    {
        string Serialize(object value);

        object Deserialize([NotNull] string input);
    }
}