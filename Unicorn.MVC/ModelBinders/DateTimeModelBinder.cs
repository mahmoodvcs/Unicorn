using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.Mvc.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return GetDateTime(value.RawValue);
        }
        public static DateTime? GetDateTime(object value)
        {
            if (value == null)
                return null;
            string s;
            if (value is string[])
                s = ((string[])value)[0];
            else
                s = value.ToString();
            if (string.IsNullOrEmpty(s) || s == "null")
                return null;
            long l;
            if (long.TryParse(s, out l))
                return new DateTime(1970, 01, 01).AddMilliseconds(l).ToLocalTime();//Convert from javascript Date.getTime()
            return (DateTime?)PersianDateTime.Parse(s);

        }
    }
    public class NullableDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return DateTimeModelBinder.GetDateTime(value.RawValue);
        }
    }
}
