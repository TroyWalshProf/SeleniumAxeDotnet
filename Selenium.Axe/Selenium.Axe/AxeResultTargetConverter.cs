using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Selenium.Axe
{
    class AxeResultTargetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(AxeResultTarget);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    return new AxeResultTarget
                    {
                        Selector = serializer.Deserialize<string>(reader)
                    };
                case JsonToken.StartArray:
                    return new AxeResultTarget
                    {
                        Selectors = serializer.Deserialize<List<string>>(reader)
                    };
            }
            
            throw new ArgumentException("Cannot unmarshal type Target");
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = (AxeResultTarget)value;
            if (val.Selector != null)
            {
                serializer.Serialize(writer, val.Selector);
                return;
            }

            if (val.Selectors != null)
            {
                serializer.Serialize(writer, val.Selectors);
                return;
            }
            
            throw new ArgumentException("Cannot marshal type Target");
        }
    }
}
