using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class IntSerializer : IContextSerializer<int>
    {
        public string Serialize(int value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public int Deserialize(string input) =>
            int.Parse(input, CultureInfo.InvariantCulture);
    }
}