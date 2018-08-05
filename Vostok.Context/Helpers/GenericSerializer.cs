namespace Vostok.Context.Helpers
{
    internal class GenericSerializer<T> : IContextSerializer
    {
        private readonly IContextSerializer<T> serializer;

        public GenericSerializer(IContextSerializer<T> serializer)
        {
            this.serializer = serializer;
        }

        public string Serialize(object value)
        {
            return serializer.Serialize((T) value);
        }

        public object Deserialize(string input)
        {
            return serializer.Deserialize(input);
        }
    }
}
