using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Unicorn.Web
{
    public static class Extensions
    {
        public static string RelativePath(this HttpServerUtility srv, string path, HttpRequest context)
        {
            return path.Replace(context.ServerVariables["APPL_PHYSICAL_PATH"], "~/").Replace(@"\", "/");
        }
    }
}
