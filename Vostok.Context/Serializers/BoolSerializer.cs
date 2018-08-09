using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class BoolSerializer : IContextSerializer<bool>
    {
        public string Serialize(bool value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public bool Deserialize(string input) =>
            bool.Parse(input);
    }
}