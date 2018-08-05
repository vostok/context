using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class FloatSerializer : IContextSerializer<float>
    {
        public string Serialize(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public float Deserialize(string input)
        {
            return float.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}