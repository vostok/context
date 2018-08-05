using System.Net;

namespace Vostok.Context.Serializers
{
    internal class IPAddressSerializer : IContextSerializer<IPAddress>
    {
        public string Serialize(IPAddress value)
        {
            return value.ToString();
        }

        public IPAddress Deserialize(string input)
        {
            return IPAddress.Parse(input);
        }
    }
}