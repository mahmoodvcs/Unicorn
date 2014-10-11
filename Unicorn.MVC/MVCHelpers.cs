using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Unicorn.MVC
{
    public static class MVCHelpers
    {
        public static UrlHelper CreateUrlHelper()
        {
            var requestContext = new System.Web.Routing.RequestContext(
               new HttpContextWrapper(HttpContext.Current), new System.Web.Routing.RouteData());
            return new System.Web.Mvc.UrlHelper(requestContext);
        }
    }
}
