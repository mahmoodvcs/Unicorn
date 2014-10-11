using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Unicorn.Web.Security
{
	public class UniHttpHandler : IHttpHandler
	{
		public const string HandlerPath = "UniSecurity.ashx";

		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			switch (context.Request.QueryString["a"])
			{
				case "cu":
					{
						CreateUserPage p = new CreateUserPage();
						((IHttpHandler)p).ProcessRequest(context);
					}
					break;
				case "conf":
					{
						ConfigurationPage p = new ConfigurationPage();
						p.BackTpPage = context.Request.QueryString["b"];
						if (context.Request.QueryString["p"] == "2")
							p.MultiView1.ActiveViewIndex = 1;
						((IHttpHandler)p).ProcessRequest(context);
					}
					break;
			}
		}

		#endregion
	}
}
