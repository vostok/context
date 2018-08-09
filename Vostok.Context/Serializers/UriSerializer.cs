using System;

namespace Vostok.Context.Serializers
{
    internal class UriSerializer : IContextSerializer<Uri>
    {
        public string Serialize(Uri value) =>
            value.ToString();

        public Uri Deserialize(string input) =>
            new Uri(input, UriKind.RelativeOrAbsolute);
    }
}