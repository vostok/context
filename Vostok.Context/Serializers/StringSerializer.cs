namespace Vostok.Context.Serializers
{
    internal class StringSerializer : IContextSerializer<string>
    {
        public string Serialize(string value) => value;

        public string Deserialize(string input) => input;
    }
}