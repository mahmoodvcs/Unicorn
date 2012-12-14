using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unicorn.Web;

namespace Unicorn.Web.Security.Authorization
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)
    , AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)
    , AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class AuthorizeActionAttribute : Attribute, IAuthorize
	{
		public AuthorizeActionAttribute(string action)
		{
            this.action = action;
		}
		private string action;

		public bool Authorize(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			IPrincipal user = httpContext.User;
			if (!user.Identity.IsAuthenticated)
			{
				return false;
			}
            return AuthorizationChecker.HasAccess(user.Identity.Name, action);
		}
	}
}
