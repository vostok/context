using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class LongSerializer : IContextSerializer<long>
    {
        public string Serialize(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public long Deserialize(string input)
        {
            return long.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}