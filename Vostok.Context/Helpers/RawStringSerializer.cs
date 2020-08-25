namespace Vostok.Context.Helpers
{
    internal class RawStringSerializer : IContextSerializer
    {
        public static readonly RawStringSerializer Instance = new RawStringSerializer();

        public string Serialize(object value) =>
            (string)value;

        public object Deserialize(string input) =>
            input;
    }
}