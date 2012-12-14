using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Unicorn.Web.UI
{
    public class ChangePassword: System.Web.UI.WebControls.ChangePassword
    {
        public ChangePassword()
        {
            //this.FailureTextStyle = "نام کاربري و يا رمز عبور اشتباه است";
			this.PasswordLabelText = "كلمه رمز :";
			this.PasswordRequiredErrorMessage = "كلمه رمز وارد نشده است";
            //this.TitleTextStyle = "ورود به سايت";
            this.UserNameLabelText = "نام کاربري :";
            this.UserNameRequiredErrorMessage = "نام کاربري وارد نشده است";
			this.CancelButtonText=" لغو ";
			this.ChangePasswordButtonText = "تغيير كلمه رمز";
			this.ChangePasswordFailureText = "كلمه رمز اشتباه و يا كلمه رمز جديد نامعتبر است. حداقل طول كلمه رمز جديد: {0}.";
			this.ChangePasswordTitleText = "تغيير كلمه رمز";
			this.ConfirmNewPasswordLabelText = "تكرار كلمه رمز جديد: ";
			this.ConfirmPasswordCompareErrorMessage = "كلمه رمز جديد و تكرار آن با هم برابر نيستند";
			this.ConfirmPasswordRequiredErrorMessage = "تكرار كلمه رمز جديد را وارد كنيد";
			this.ContinueButtonText = "ادامه";
			//this.CreateUserText = "ايجاد كاربر جديد";
			this.NewPasswordLabelText = "كلمه رمز جديد: ";
			this.NewPasswordRegularExpressionErrorMessage = "لطفا كلمه رمز ديگري وارد نماييد.";
			this.NewPasswordRequiredErrorMessage = "كلمه رمز جديد وارد نشده است.";
			this.SuccessText = "كلمه رمز جديد با موفقيت جايگزين شد.";
			//this.SuccessTitleText = "كلمه رمز جديد با موفقيت جايگزين شد.";
            Attributes["dir"] = "rtl";
        }
        //static ChangePassword()
        //{
        //    ConfigInitializer.Initialize();
        //}
    }
}
