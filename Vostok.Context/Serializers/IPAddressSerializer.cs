using System.Net;

namespace Vostok.Context.Serializers
{
    internal class IPAddressSerializer : IContextSerializer<IPAddress>
    {
        public string Serialize(IPAddress value) =>
            value.ToString();

        public IPAddress Deserialize(string input) =>
            IPAddress.Parse(input);
    }
}