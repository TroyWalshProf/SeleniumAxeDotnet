using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Selenium.Axe
{
    class AxeResultTargetConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AxeResultTarget);
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
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
            
            throw new Exception("Cannot unmarshal type Target");
        }
        
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (AxeResultTarget)untypedValue;
            if (value.Selector != null)
            {
                serializer.Serialize(writer, value.Selector);
                return;
            }

            if (value.Selectors != null)
            {
                serializer.Serialize(writer, value.Selectors);
                return;
            }
            
            throw new Exception("Cannot marshal type Target");
        }
    }
}