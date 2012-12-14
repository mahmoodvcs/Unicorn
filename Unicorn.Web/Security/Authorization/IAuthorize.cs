using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unicorn.Web.Security.Authorization
{
	public interface IAuthorize
	{
		bool Authorize(HttpContext httpContext);
	}
}
