using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class ShortSerializer : IContextSerializer<short>
    {
        public string Serialize(short value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public short Deserialize(string input)
        {
            return short.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}