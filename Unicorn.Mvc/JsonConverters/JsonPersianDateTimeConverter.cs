using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unicorn.Web;

namespace Unicorn.Mvc
{
    public class JsonPersianDateTimeConverter : Newtonsoft.Json.Converters.DateTimeConverterBase
    {
        private bool ConvertTime;
        private bool ArabicNumerals;

        public JsonPersianDateTimeConverter(bool includeTime = false, bool arabicNumerals = false)
        {
            // TODO: Complete member initialization
            this.ConvertTime = includeTime;
            this.ArabicNumerals = arabicNumerals;
        }
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DateTime) || objectType == typeof(DateTime?));
        }
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
                return;
            var dt = (Unicorn.PersianDateTime)(DateTime)value;
            string val;
            if (ConvertTime)
                val = dt.ToShortDateTimeString();
            else
                val = dt.ToShortDateString();
            if (ArabicNumerals)
                val = val.ToArabicNumerals();
            writer.WriteValue(val);
        }
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            var v = reader.Value.ToString().Trim();
            if ( v == "")
                return null;
            return Unicorn.PersianDateTimeConverter.ShamsiToMiladi(Unicorn.PersianDateTime.Parse(v));
        }
    }
}