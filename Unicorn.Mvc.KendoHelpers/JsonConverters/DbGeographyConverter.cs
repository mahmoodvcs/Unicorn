using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity.Spatial;

namespace Unicorn.Mvc.KendoHelpers.JsonConverters
{
    public class DbGeographyConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DbGeography));
        }
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var j = JObject.Parse(reader.ReadAsString());
            JToken lng, lat;
            if (!j.TryGetValue("Lon", out lng) || !j.TryGetValue("Lat", out lat))
                throw new Exception("Invalid json.");
            DbGeography geo = DbGeography.FromText(string.Format("POINT({0} {1})", lng.ToObject<double>(), lat.ToObject<double>()));
            return geo;
        }
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            DbGeography geo=value as DbGeography;
            if(geo==null)
                throw new ArgumentException("Parameter is not a DbGeography", "value");
            writer.WriteStartObject();
            writer.WritePropertyName("Lon");
            writer.WriteValue(geo.Longitude);
            writer.WritePropertyName("Lat");
            writer.WriteValue(geo.Latitude);
            writer.WriteEndObject();
        }
    }
}
