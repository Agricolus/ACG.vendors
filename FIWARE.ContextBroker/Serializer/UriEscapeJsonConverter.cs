using System;
using Newtonsoft.Json;

namespace FIWARE.ContextBroker.Serializer
{
    public class UriEscapeJsonConverter : JsonConverter
    {
        private char[] ForbiddenChars = null; //new char[] { '<', '>', '"', '\'', '=', ';', '(', ')' };
        public UriEscapeJsonConverter() { }

        public UriEscapeJsonConverter(params char[] EscapeOnly)
        {
            this.ForbiddenChars = EscapeOnly;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) return;
            var encodedValue = "";
            var val = (string)value;
            if (this.ForbiddenChars != null)
            {
                var chars = val.ToCharArray();
                foreach (var fc in this.ForbiddenChars)
                {
                    val = val.Replace(fc.ToString(), Uri.HexEscape(fc));
                }
                encodedValue = val;
            }
            else
            {
                encodedValue = Uri.EscapeDataString(val);
            }
            writer.WriteValue(encodedValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            var decodedValue = Uri.UnescapeDataString(reader.Value.ToString());
            return decodedValue;
        }
    }
}
