namespace Vostok.Context.Serializers
{
    internal class StringSerializer : IContextSerializer<string>
    {
        public string Serialize(string value)
        {
            return value;
        }

        public string Deserialize(string input)
        {
            return input;
        }
    }
}
