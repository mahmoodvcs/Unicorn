using System.Web.Mvc;

namespace Unicorn.Mvc.Filters
{
    public class JsonExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            filterContext.HttpContext.Response.Write(filterContext.Exception.Message);
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.ExceptionHandled = true;
        }
    }
}