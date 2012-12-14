using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Web.Security;
using System.Data;
using Unicorn.Web.Security;

namespace Unicorn.Web.UI
{
    public class EditUserRoles : CompositeControl
    {
        CheckBoxList uxRoles;
        Button uxSave;
        public event EventHandler SaveClicked;

        [Category("Behavior")]
        public string UserName
        {
            get { return (string)ViewState["u"]; }
            set { ViewState["u"] = value; }
        }
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "4");
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            uxRoles = new CheckBoxList();
            uxRoles.DataValueField = "RoleName";
            uxRoles.DataTextField = "RoleTitle";
            uxRoles.DataSource = UniRoles.GetAllRoles();
            uxRoles.DataBind();
            Controls.Add(uxRoles);
            uxSave = new Button();
            uxSave.Text = "ذخيره";
            uxSave.Click += new EventHandler(uxSave_Click);
            Controls.Add(uxSave);
            ChildControlsCreated = true;
        }

        private DataTable GetAllRoles()
        {
            return Unicorn.Data.SqlHelper.ExecuteCommand("Select * from aspnet_Roles");
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.WriteEncodedText("نام كاربر: ");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.WriteEncodedText(string.IsNullOrEmpty(UserName) ? "-" : UserName);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.WriteEncodedText("نقشها: ");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            if (!string.IsNullOrEmpty(UserName))
            {
                string[] roles = Roles.GetRolesForUser(UserName);
                foreach (string r in roles)
                {
                    uxRoles.Items.FindByValue(r).Selected = true;
                }
            }
            uxRoles.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            uxSave.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        void uxSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                string[] roles = Roles.GetRolesForUser(UserName);
                if (roles.Length > 0)
                    Roles.RemoveUserFromRoles(UserName, roles);
                List<string> newRoles = new List<string>();
                foreach (ListItem l in uxRoles.Items)
                {
                    if (l.Selected)
                        newRoles.Add(l.Value);
                }
                if (newRoles.Count > 0)
                    Roles.AddUserToRoles(UserName, newRoles.ToArray());
            }
            if (SaveClicked != null)
                SaveClicked(this, EventArgs.Empty);
        }
    }
}
