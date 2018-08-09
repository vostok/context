namespace Vostok.Context.Serializers
{
    internal class CharSerializer : IContextSerializer<char>
    {
        public string Serialize(char value) =>
            value.ToString();

        public char Deserialize(string input) =>
            char.Parse(input);
    }
}