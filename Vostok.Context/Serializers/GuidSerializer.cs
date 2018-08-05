using System;

namespace Vostok.Context.Serializers
{
    internal class GuidSerializer : IContextSerializer<Guid>
    {
        public string Serialize(Guid value)
        {
            return value.ToString("D");
        }

        public Guid Deserialize(string input)
        {
            return Guid.Parse(input);
        }
    }
}