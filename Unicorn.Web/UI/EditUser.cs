using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:JcoEditUser runat=server></{0}:JcoEditUser>")]
    public class EditUser : CompositeControl
    {
        public EditUser()
        {
        }
        Button uxSave,uxCancel;
        Label lblError;
        TextBox uxUserName, uxPassword, uxConfirmPassword, uxEMail, uxOldPassword;
        RequiredFieldValidator valUserName, valPassword, valConfirmPassword, valEMail;
        List<Control> controls;

    }
}
