using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class SignedByteSerializer : IContextSerializer<sbyte>
    {
        public string Serialize(sbyte value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public sbyte Deserialize(string input) =>
            sbyte.Parse(input, CultureInfo.InvariantCulture);
    }
}