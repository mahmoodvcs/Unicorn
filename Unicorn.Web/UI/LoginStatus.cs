using System;
using System.Collections.Generic;
using System.Text;
using Unicorn.Web.Security.Configuration;

namespace Unicorn.Web.UI
{
	public class LoginStatus : System.Web.UI.WebControls.LoginStatus
	{
		public LoginStatus()
		{
			LoginText = "ورود";
			LogoutText = "خروج";
		}
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			if ( !DesignMode )
				ConfigInitializer.CheckConfig( this );
		}
	}
}
