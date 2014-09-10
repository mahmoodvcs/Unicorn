using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unicorn.Web.Security.Authorization;

namespace Unicorn.MVC
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeActionAttribute : AuthorizeAttribute
    {
        public AuthorizeActionAttribute(string action)
        {
            this.action = action;
        }
        string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (AuthorizationChecker.HasAccess(httpContext.User.Identity.Name, action))
                return true;
            return base.AuthorizeCore(httpContext);
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }

}