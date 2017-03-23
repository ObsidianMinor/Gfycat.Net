using Newtonsoft.Json;
using System;
using System.Net;

namespace Gfycat.Converters
{
    internal class ExceptionMessageConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(API.ExceptionMessage);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.StartObject)
            {
                string code = null, description = null;
                while(reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    switch(reader.Value)
                    {
                        case "code":
                            code = reader.ReadAsString();
                            break;
                        case "description":
                            description = reader.ReadAsString();
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }

                return new API.ExceptionMessage() { Code = code, Description = description };
            }
            else
                return new API.ExceptionMessage() { Description = WebUtility.UrlDecode(reader.Value.ToString()) };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }
    }
}
