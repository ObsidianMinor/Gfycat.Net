using Newtonsoft.Json;
using System;

namespace Gfycat
{
    public class UnixTimeConverter : JsonConverter
    {
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(long);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _epoch.AddSeconds((long)existingValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((((DateTime)value).ToUniversalTime() - _epoch).TotalSeconds);
        }
    }
}
