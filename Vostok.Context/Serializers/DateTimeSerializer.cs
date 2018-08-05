using System;
using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class DateTimeSerializer : IContextSerializer<DateTime>
    {
        public string Serialize(DateTime value)
        {
            return value.ToString("O", CultureInfo.InvariantCulture);
        }

        public DateTime Deserialize(string input)
        {
            return DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}