﻿using Newtonsoft.Json;
using System;

namespace Gfycat.Converters
{
    internal class NumericalBooleanConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString() == "1";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((bool)value ? 1 : 0);
        }
    }
}
