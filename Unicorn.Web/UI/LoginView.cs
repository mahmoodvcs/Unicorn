using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web.Security.Configuration;

namespace Unicorn.Web.UI
{
	public class LoginView : System.Web.UI.WebControls.LoginView
	{
		public LoginView()
		{
			this.AnonymousTemplate = new LoginViewAnonymousTemplate();
			this.LoggedInTemplate = new LoginViewLoggedInTemplate();
		}
		//static LoginView()
		//{
		//    ConfigInitializer.Initialize();
		//}
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			if ( !DesignMode )
				ConfigInitializer.CheckConfig( this );
		}
	}

	internal class LoginViewAnonymousTemplate : ITemplate
	{
		public void InstantiateIn( Control container )
		{
			container.Controls.Add( new LoginStatus() );
		}
	}
	internal class LoginViewLoggedInTemplate : ITemplate
	{
		public void InstantiateIn( Control container )
		{
			LiteralControl l = new LiteralControl( "خوش آمديد " );
			container.Controls.Add( l );
			LoginName ln = new LoginName();
			container.Controls.Add( ln );
			l = new LiteralControl( " | " );
			container.Controls.Add( l );
			container.Controls.Add( new LoginStatus() );
		}
	}
}
