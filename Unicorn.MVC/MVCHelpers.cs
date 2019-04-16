using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Unicorn.Mvc
{
    public static class MVCHelpers
    {
        public static UrlHelper CreateUrlHelper()
        {
            var requestContext = new System.Web.Routing.RequestContext(
               new HttpContextWrapper(HttpContext.Current), new System.Web.Routing.RouteData());
            return new System.Web.Mvc.UrlHelper(requestContext);
        }
        public static void ClearErrors(this ModelStateDictionary modelState, params string[] fields)
        {
            if (fields.Length == 0)
            {
                foreach (var k in modelState.Keys)
                    modelState[k].Errors.Clear();
            }
            foreach (var f in fields)
            {
                if (modelState.ContainsKey(f) && modelState[f].Errors.Count > 0)
                    modelState[f].Errors.Clear();
            }
        }
    }
}
