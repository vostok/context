using System;
using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DateTimeOffsetSerializer : IContextSerializer<DateTimeOffset>
    {
        public string Serialize(DateTimeOffset value)
        {
            return value.ToString("O", CultureInfo.InvariantCulture);
        }

        public DateTimeOffset Deserialize(string input)
        {
            return DateTimeOffset.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}