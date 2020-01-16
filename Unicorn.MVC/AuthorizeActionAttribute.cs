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
        public AuthorizeActionAttribute(params string[] actions)
        {
            this.actions = actions;
            if (!string.IsNullOrEmpty(AdministratorRoles))
                if (string.IsNullOrEmpty(base.Roles))
                    base.Roles = AdministratorRoles;
                else
                    base.Roles += "," + AdministratorRoles;

        }
        string[] actions;

        public string[] Actions
        {
            get { return actions; }
            set { actions = value; }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
                return false;
            foreach (var ac in actions)
            {
                var access = AuthorizationChecker.HasAccess(httpContext.User.Identity.Name, ac);
                if (access)
                    return true;
            }
            if (string.IsNullOrEmpty(Roles) && string.IsNullOrEmpty(Users))
                return false;

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