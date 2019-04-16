using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unicorn.Mvc.JsonConverters
{
    public class JsDateTimeConverter : Newtonsoft.Json.Converters.DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            var v = reader.Value.ToString().Trim();
            if (v == "")
                return null;
            if (long.TryParse(v, out var l))
                return new DateTime(1970, 01, 01).AddMilliseconds(l).ToLocalTime();
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;
            var dt = (DateTime)value;
            var l = (dt - new DateTime(1970, 01, 01)).TotalMilliseconds;
            writer.WriteValue(l);
        }
    }
}
