using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Unicorn.Mvc.ModelBinders
{
    public class MyByteArrayModelBinder : System.Web.Mvc.ByteArrayModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var file = controllerContext.HttpContext.Request.Files[bindingContext.ModelName];

            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, fileBytes.Length);
                    return fileBytes;
                }

                return null;
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
