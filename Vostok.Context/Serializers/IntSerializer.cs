using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class IntSerializer : IContextSerializer<int>
    {
        public string Serialize(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public int Deserialize(string input)
        {
            return int.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}