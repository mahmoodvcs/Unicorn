using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web.Security.Configuration;

namespace Unicorn.Web.UI
{
	public class Login : System.Web.UI.WebControls.Login
	{
		public Login()
		{
			this.LoginButtonText = "ورود";
			//this.CreateUserText = "ايجاد کاربر";
			this.FailureText = "نام کاربري و يا كلمه رمز اشتباه است";
			//this.HelpPageText
			//this.InstructionText=
			//this.PasswordRecoveryText = "";
			this.PasswordLabelText = "كلمه رمز :";
			this.PasswordRequiredErrorMessage = "كلمه رمز وارد نشده است";
			this.RememberMeText = "من را به خاطر بسپار";
			this.TitleText = "ورود به سايت";
			this.UserNameLabelText = "نام کاربري :";
			this.UserNameRequiredErrorMessage = "نام کاربري وارد نشده است";
			Attributes["dir"] = "rtl";
		}
		//static Login()
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
}
