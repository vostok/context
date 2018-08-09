using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class UnsignedShortSerializer : IContextSerializer<ushort>
    {
        public string Serialize(ushort value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public ushort Deserialize(string input) =>
            ushort.Parse(input, CultureInfo.InvariantCulture);
    }
}