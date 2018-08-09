using System;

namespace Vostok.Context.Serializers
{
    internal class GuidSerializer : IContextSerializer<Guid>
    {
        public string Serialize(Guid value) =>
            value.ToString("D");

        public Guid Deserialize(string input) =>
            Guid.Parse(input);
    }
}