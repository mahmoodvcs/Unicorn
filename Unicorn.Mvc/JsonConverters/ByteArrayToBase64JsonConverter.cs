using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mvc
{
    public class ByteArrayToBase64JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bs = value as byte[];
            if (bs == null)
                writer.WriteValue((object)null);
            else
            {
                //writer.WritePropertyName(writer.Path);
                writer.WriteValue(Convert.ToBase64String(bs));
            }
        }
    }
}
