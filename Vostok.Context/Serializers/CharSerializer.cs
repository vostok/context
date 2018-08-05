namespace Vostok.Context.Serializers
{
    internal class CharSerializer : IContextSerializer<char>
    {
        public string Serialize(char value)
        {
            return value.ToString();
        }

        public char Deserialize(string input)
        {
            return char.Parse(input);
        }
    }
}