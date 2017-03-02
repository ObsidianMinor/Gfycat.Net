using Newtonsoft.Json;
using System;

namespace Gfycat.Converters
{
    public class UnixTimeConverter : JsonConverter
    {
        DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _epoch.AddSeconds(ulong.Parse((string)reader.Value));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((ulong)(((DateTime)value).ToUniversalTime() - _epoch).TotalSeconds);
        }
    }
}
