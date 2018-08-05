using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class UnsignedLongSerializer : IContextSerializer<ulong>
    {
        public string Serialize(ulong value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public ulong Deserialize(string input)
        {
            return ulong.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}