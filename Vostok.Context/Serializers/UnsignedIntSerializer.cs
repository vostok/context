using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class UnsignedIntSerializer : IContextSerializer<uint>
    {
        public string Serialize(uint value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public uint Deserialize(string input)
        {
            return uint.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}