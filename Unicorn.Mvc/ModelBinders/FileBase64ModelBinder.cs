using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Unicorn.Mvc.ModelBinders
{
    public class FileBase64ModelBinder : MyByteArrayModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
                return null;
            if (value.RawValue is byte[])
                return value.RawValue;
            if (value.RawValue is string)
            {
                try
                {
                    var val = (string)value.RawValue;
                    val = val.Replace(" ", "+");
                    int mod4 = val.Length % 4;
                    if (mod4 > 0)
                    {
                        val += new string('=', 4 - mod4);
                    }
                    return Convert.FromBase64String(val);
                }
                catch
                {
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}