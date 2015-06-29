using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Unicorn.Web.Security.Authorization;

namespace Unicorn.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeActionAttribute : AuthorizeAttribute
    {
        public static string UnauthorizedRedirectAction { get; set; }
        public static string UnauthorizedRedirectController { get; set; }
        public static string AdministratorRoles { get; set; }
        public AuthorizeActionAttribute(string action)
        {
            this.action = action;
            if (!string.IsNullOrEmpty(AdministratorRoles))
                if (string.IsNullOrEmpty(base.Roles))
                    base.Roles = AdministratorRoles;
                else
                    base.Roles += "," + AdministratorRoles;

        }
        string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
                return false;
            var access = AuthorizationChecker.HasAccess(httpContext.User.Identity.Name, action);
            if (access || (string.IsNullOrEmpty(Roles) && string.IsNullOrEmpty(Users)))
                return access;

            return base.AuthorizeCore(httpContext);
        }
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    base.OnAuthorization(filterContext);
        //}
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(UnauthorizedRedirectAction) && !string.IsNullOrEmpty(UnauthorizedRedirectController))
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    controller = UnauthorizedRedirectController,
                                    action = UnauthorizedRedirectAction
                                })
                            );
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }

}