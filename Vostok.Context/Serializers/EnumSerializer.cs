using System;

namespace Vostok.Context.Serializers
{
    internal class EnumSerializer<T> : IContextSerializer<T>
    {
        public EnumSerializer()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"Type {typeof(T).Name} is not an enum.");
        }

        public string Serialize(T value) =>
            value.ToString();

        public T Deserialize(string input) =>
            (T)Enum.Parse(typeof(T), input, true);
    }
}