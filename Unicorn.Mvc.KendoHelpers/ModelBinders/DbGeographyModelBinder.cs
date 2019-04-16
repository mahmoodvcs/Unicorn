using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.Mvc.KendoHelpers.ModelBinders
{
    public class DbGeographyModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object latitude = null, longitude = null;
            var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (val == null || !TryDeserializeJson(val.RawValue, out latitude, out longitude))
            {
                val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Lat");
                if (val == null)
                    val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Latitude");
                if (val == null)
                    return null;
                latitude = val.RawValue;
                val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Lon");
                if (val == null)
                    val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Longitude");
                if (val == null)
                    return null;
                longitude = val.RawValue;
            }
            if (latitude != null && longitude != null)
            {
                var text = string.Format("POINT({0} {1})", longitude, latitude);
                // 4326 is most common coordinate system used by GPS/Maps
                return DbGeography.PointFromText(text, 4326);
            }
            return null;
        }
        bool TryDeserializeJson(object rawValue, out object latitude, out object longitude)
        {
            latitude = null;
            longitude = null;
            if (rawValue is string[])
                rawValue = ((string[])rawValue)[0];
            if (!(rawValue is string))
                return false;
            dynamic obj = JsonConvert.DeserializeObject(rawValue.ToString());
            try
            {
                latitude = obj.Latitude;
                longitude = obj.Longitude;
            }
            catch { }
            return latitude != null;
        }
    }
}
