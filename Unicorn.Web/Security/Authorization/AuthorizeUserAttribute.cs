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
	public class AuthorizeUserAttribute : Attribute, IAuthorize
	{
		public AuthorizeUserAttribute(string users)
		{
			Users = users;
		}
		private string users;
		private string[] usersSplit = new string[0];


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
			if (this.usersSplit.Length > 0 && !Contains(this.usersSplit, user.Identity.Name))
			{
				return false;
			}
			return true;
		}
		public string Users
		{
			get
			{
				return (this.users ?? string.Empty);
			}
			set
			{
				this.users = value;
				this.usersSplit = Utility.SplitString(value, ',');
			}
		}

		private bool Contains(string[] array, string item)
		{
			foreach (string s in array)
				if (StringComparer.OrdinalIgnoreCase.Compare(s, item) == 0)
					return true;
			return false;
		}

	}
}
