using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DecimalSerializer : IContextSerializer<decimal>
    {
        public string Serialize(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public decimal Deserialize(string input)
        {
            return decimal.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}