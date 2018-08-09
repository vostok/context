using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DoubleSerializer : IContextSerializer<double>
    {
        public string Serialize(double value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public double Deserialize(string input) =>
            double.Parse(input, CultureInfo.InvariantCulture);
    }
}