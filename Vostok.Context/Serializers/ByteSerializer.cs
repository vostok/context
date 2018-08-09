using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class ByteSerializer : IContextSerializer<byte>
    {
        public string Serialize(byte value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public byte Deserialize(string input) =>
            byte.Parse(input, CultureInfo.InvariantCulture);
    }
}