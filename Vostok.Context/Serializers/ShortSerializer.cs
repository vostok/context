using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class ShortSerializer : IContextSerializer<short>
    {
        public string Serialize(short value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public short Deserialize(string input) =>
            short.Parse(input, CultureInfo.InvariantCulture);
    }
}