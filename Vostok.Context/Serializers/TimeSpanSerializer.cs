using System;
using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class TimeSpanSerializer : IContextSerializer<TimeSpan>
    {
        public string Serialize(TimeSpan value)
        {
            return value.ToString("G", CultureInfo.InvariantCulture);
        }

        public TimeSpan Deserialize(string input)
        {
            return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}