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
            var latitude = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Lat").RawValue;
            var longitude = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".Lon").RawValue;
            if (latitude != null && longitude != null)
            {
                var text = string.Format("POINT({0} {1})", longitude, latitude);
                // 4326 is most common coordinate system used by GPS/Maps
                return DbGeography.PointFromText(text, 4326);
            }
            return null;
        }
    }
}
