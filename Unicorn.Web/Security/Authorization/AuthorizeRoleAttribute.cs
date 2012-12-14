using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unicorn.Web;

namespace Unicorn.Web.Security.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class AuthorizeRoleAttribute : Attribute, IAuthorize
	{
		public AuthorizeRoleAttribute(string roles)
		{
			Roles = roles;
		}
		private string roles;
		private string[] rolesSplit = new string[0];

		public string Roles
		{
			get
			{
				return (this.roles ?? string.Empty);
			}
			set
			{
				this.roles = value;
				this.rolesSplit = Utility.SplitString(value, ',');
			}
		}
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
			if ((this.rolesSplit.Length > 0) && 
				!Array.Exists<string>(this.rolesSplit, new Predicate<string>(user.IsInRole)))
			{
				return false;
			}
			return true;
		}
	}
}