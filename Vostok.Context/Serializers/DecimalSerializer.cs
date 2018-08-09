using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DecimalSerializer : IContextSerializer<decimal>
    {
        public string Serialize(decimal value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public decimal Deserialize(string input) =>
            decimal.Parse(input, CultureInfo.InvariantCulture);
    }
}