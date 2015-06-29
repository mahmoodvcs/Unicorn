using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Unicorn.Mvc.ModelBinders
{
    public class IntArrayModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
            {
                return null;
            }

            return value
                .AttemptedValue
                .Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
        }
    }
}