using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class UnsignedShortSerializer : IContextSerializer<ushort>
    {
        public string Serialize(ushort value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public ushort Deserialize(string input)
        {
            return ushort.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}