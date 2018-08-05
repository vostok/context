using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class BoolSerializer : IContextSerializer<bool>
    {
        public string Serialize(bool value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public bool Deserialize(string input)
        {
            return bool.Parse(input);
        }
    }
}