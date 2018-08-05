using System;

namespace Vostok.Context.Serializers
{
    internal class UriSerializer : IContextSerializer<Uri>
    {
        public string Serialize(Uri value)
        {
            return value.ToString();
        }

        public Uri Deserialize(string input)
        {
            return new Uri(input, UriKind.RelativeOrAbsolute);
        }
    }
}