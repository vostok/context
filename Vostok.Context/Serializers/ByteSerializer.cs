using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class ByteSerializer : IContextSerializer<byte>
    {
        public string Serialize(byte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public byte Deserialize(string input)
        {
            return byte.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}