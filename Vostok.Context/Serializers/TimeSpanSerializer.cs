using System;
using System.Globalization;

namespace Vostok.Context.Serializers
{
    internal class TimeSpanSerializer : IContextSerializer<TimeSpan>
    {
        public string Serialize(TimeSpan value) =>
            value.ToString("G", CultureInfo.InvariantCulture);

        public TimeSpan Deserialize(string input) =>
            TimeSpan.Parse(input, CultureInfo.InvariantCulture);
    }
}