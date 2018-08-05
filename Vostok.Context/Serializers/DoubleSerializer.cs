using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DoubleSerializer : IContextSerializer<double>
    {
        public string Serialize(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public double Deserialize(string input)
        {
            return double.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}