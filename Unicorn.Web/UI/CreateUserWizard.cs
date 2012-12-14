using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.ComponentModel;
using Unicorn.Web.Security;

namespace Unicorn.Web.UI
{
    public class CreateUserWizard : System.Web.UI.WebControls.CreateUserWizard
    {
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool EnableSelectRoles
        {
            get
            {
                object o = ViewState["EnableSelectRoles"];
                if (o != null)
                    return (bool)o;
                return true;
            }
            set { ViewState["EnableSelectRoles"] = value; }
        }

        public CreateUserWizard()
        {
            //this.HeaderText = "";
            this.CancelButtonText = "لغو";
            this.ContinueButtonText = "ادامه";
            this.FinishCompleteButtonText = "تاييد";
            this.FinishPreviousButtonText = "قبلي";
            //CompleteSuccessText = "ايجاد شد";
            //Email:
            this.EmailLabelText = "ايميل :";
            this.EmailRegularExpressionErrorMessage = "لطفا آدرس ايميل ديگري وارد كنيد";
            this.DuplicateEmailErrorMessage = "آدرس ايميل وارد شده قبلا استفاده شده است. لطفا آدرس ايمبل ديگري وارد كنيد.";
            this.EmailRequiredErrorMessage = "آدرس ايميل وارد نشده است";
            this.InvalidEmailErrorMessage = "آدرس ايميل اشتباه است";

            this.UserNameLabelText = "نام کاربري :";
            this.UserNameRequiredErrorMessage = "نام كاربري وارد نشده است";
            this.DuplicateUserNameErrorMessage = "اين نام كاربري قبلا استفاده شده است. لطفا نام كاربري ديگري انتخاب كنيد.";

            this.PasswordLabelText = "كلمه رمز :";
            this.PasswordRegularExpressionErrorMessage = "لطفا كلمه رمز ديگري وارد كنيد";
            this.PasswordRequiredErrorMessage = "كلمه رمز وارد نشده است";
            this.InvalidPasswordErrorMessage = "كلمه رمز اشتباه است";
            this.ConfirmPasswordCompareErrorMessage = "كلمه رمز و تكرار آن با هم برابر نيستند";
            this.ConfirmPasswordLabelText = "تكرار كلمه رمز :";
            this.ConfirmPasswordRequiredErrorMessage = "تكرار كلمه رمز را وارد كنيد";

            this.QuestionLabelText = "سؤال امنيتي";
            this.QuestionRequiredErrorMessage = "سؤال امنيتي وارد نشده است";
            this.AnswerLabelText = "جواب امنيتي :";
            this.AnswerRequiredErrorMessage = "جواب امنيتي وارد نشده است";
            this.InvalidAnswerErrorMessage = "جواب امنيتي اشتباه است";
            this.InvalidQuestionErrorMessage = "سؤال امنيتي اشتباه است";

            this.StartNextButtonText = "بعدي";
            this.StepNextButtonText = "بعدي";
            this.StepPreviousButtonText = "قبلي";
            this.UnknownErrorMessage = "اشكالي در هنگام ايجاد كاربر بوجود آمده است.";
            this.CreateUserButtonText = "ايجاد كاربر";
            this.CompleteSuccessText = "حساب كاربري با موفقيت ايجاد شد.";

            this.LoginCreatedUser = false;

            Attributes["dir"] = "rtl";
            this.ContinueButtonClick += new EventHandler(CreateUserWizard_ContinueButtonClick);
        }

        void CreateUserWizard_ContinueButtonClick(object sender, EventArgs e)
        {
            CheckBoxList lst = (CheckBoxList)ControlUtility.FindControl(this, "uxRoles");
            List<string> roles = new List<string>();
            foreach (ListItem r in lst.Items)
            {
                if (r.Selected)
                    roles.Add(r.Value);
            }
            if (roles.Count > 0)
                Roles.AddUserToRoles(UserName, roles.ToArray());
            this.ActiveStepIndex = 0;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.WizardSteps[0].Title = "ايجاد حساب كابري جديد";
            if (((CompleteWizardStep)WizardSteps[1]).ContentTemplate == null && EnableSelectRoles)
            {
                this.WizardSteps[1].Title = "انتخاب نقشهاي کاربر";
                ((CompleteWizardStep)WizardSteps[1]).ContentTemplate = new SelectRolesTemplate();
            }
        }
        //static CreateUserWizard()
        //{
        //    ConfigInitializer.Initialize();
        //}
        private class SelectRolesTemplate : ITemplate
        {
            //public SelectRolesTemplate(CreateUserWizard w)
            //{
            //	wizard = w;
            //}
            //CreateUserWizard wizard;

            public void InstantiateIn(Control container)
            {
                if (!Roles.Enabled)
                    return;
                CheckBoxList uxRoles;
                Button ContinueButton;
                //_tbl
                HtmlTable _tbl = new HtmlTable();
                _tbl.Attributes["border"] = @"0";
                //_row
                HtmlTableRow _row = new HtmlTableRow();
                //_cell
                HtmlTableCell _cell = new HtmlTableCell();
                _cell.Attributes["align"] = @"center";
                _cell.Attributes["colspan"] = @"2";
                //_literal
                Literal _literal = new Literal();
                _literal.Text = @"حساب كاربري با موفقيت ايجاد شد.";
                _cell.Controls.Add(_literal);
                //_ctl1
                HtmlGenericControl _ctl1 = new HtmlGenericControl();
                _ctl1.TagName = @"br";
                _cell.Controls.Add(_ctl1);
                //_literal
                _literal = new Literal();
                _literal.Text = @"نقشهاي کاربر را انتخاب کنبد:";
                _cell.Controls.Add(_literal);
                _row.Controls.Add(_cell);
                _tbl.Controls.Add(_row);
                //_row
                _row = new HtmlTableRow();
                //_cell
                _cell = new HtmlTableCell();
                //uxRoles
                uxRoles = new CheckBoxList();
                uxRoles.ID = "uxRoles";
                _cell.Controls.Add(uxRoles);
                _row.Controls.Add(_cell);
                _tbl.Controls.Add(_row);
                //_row
                _row = new HtmlTableRow();
                //_cell
                _cell = new HtmlTableCell();
                _cell.Attributes["align"] = "right";
                _cell.Attributes["colspan"] = "2";
                //ContinueButton
                ContinueButton = new Button();
                ContinueButton.ID = "FinishButton";
                ContinueButton.CausesValidation = false;
                ContinueButton.CommandName = ContinueButtonCommandName;// @"Finish";
                ContinueButton.Text = "تاييد";
                ContinueButton.ValidationGroup = "CreateUserWizard1";
                _cell.Controls.Add(ContinueButton);
                _row.Controls.Add(_cell);
                _tbl.Controls.Add(_row);
                container.Controls.Add(_tbl);
                ShowAllRoles(uxRoles);
                //uxRoles.DataSource = Roles.GetAllRoles();
                //uxRoles.DataBind();
            }

            private void ShowAllRoles(CheckBoxList uxRoles)
            {
                UniRole[] roles = UniRoles.GetAllRoles();// Roles.GetAllRoles();
                foreach (UniRole role in roles)
                    uxRoles.Items.Add(new ListItem(role.RoleTitle, role.RoleName));
            }
        }
    }
}
