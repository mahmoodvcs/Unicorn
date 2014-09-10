using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web.Security.Configuration;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web.Configuration;
using Unicorn.Web.UI;
using System.Web.Security;
using System.Data.SqlClient;

namespace Unicorn.Web.Security.Pages
{
    public class ConfigurationPage : PageBase
    {
        public ConfigurationPage()
        {
            HtmlGenericControl style1 = new HtmlGenericControl();
            style1.TagName = @"style";
            style1.Attributes["type"] = @"text/css";
            style1.InnerText = @"body, input, .style1
    {
        font-family:Tahoma;
        font-size:9pt;
    }";
            Header.Controls.Add(style1);

            //MultiView1
            MultiView1 = new MultiView();
            MultiView1.ActiveViewIndex = 0;
            //view1
            view1 = new View();
            //_tbl
            HtmlTable _tbl = new HtmlTable();
            _tbl.Attributes["border"] = @"1";
            //_row
            HtmlTableRow _row = new HtmlTableRow();
            //_cell
            HtmlTableCell _cell = new HtmlTableCell();
            _cell.Attributes["colspan"] = @"2";
            _cell.Attributes["style"] = @"height: 39px";
            //_literal
            Literal _literal = new Literal();
            _literal.Text = @"
                            تنظيمات امنيتي وب سايت:
                        ";
            _cell.Controls.Add(_literal);
            _row.Controls.Add(_cell);
            _tbl.Controls.Add(_row);
            //_row
            _row = new HtmlTableRow();
            //_cell
            _cell = new HtmlTableCell();
            _cell.Attributes["style"] = @"width: 235px";
            //_literal
            _literal = new Literal();
            _literal.Text = @"
                            محل ذخيره سازي اطلاعات کاربران:
                        ";
            _cell.Controls.Add(_literal);
            _row.Controls.Add(_cell);
            //_cell
            _cell = new HtmlTableCell();
            //uxProviders
            uxProviders = new RadioButtonList();
            uxProviders.RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Horizontal;
            uxProviders.Width = new Unit("385px");
            uxProviders.AutoPostBack = true;
            uxProviders.CssClass = @"style1";
            uxProviders.SelectedIndexChanged += new System.EventHandler(uxProviders_SelectedIndexChanged);
            //_item
            ListItem _item = new ListItem();
            _item.Selected = true;
            _item.Value = @"sql";
            _item.Text = @"Sql Server Database";
            uxProviders.Items.Add(_item);
            //_item
            _item = new ListItem();
            _item.Value = @"access";
            _item.Text = @"MS Access Database";
            uxProviders.Items.Add(_item);
            _cell.Controls.Add(uxProviders);
            _row.Controls.Add(_cell);
            _tbl.Controls.Add(_row);
            //trSql
            trSql = new HtmlTableRow();
            //_cell
            _cell = new HtmlTableCell();
            //_literal
            _literal = new Literal();
            _literal.Text = @"
                            رشته اتصال:
                        ";
            _cell.Controls.Add(_literal);
            trSql.Controls.Add(_cell);
            //_cell
            _cell = new HtmlTableCell();
            _cell.Attributes["dir"] = @"ltr";
            //uxConnectionStrings
            uxConnectionStrings = new RadioButtonList();
            uxConnectionStrings.CssClass = @"style1";
            uxConnectionStrings.Font.Size = new FontUnit("9pt");
            _cell.Controls.Add(uxConnectionStrings);
            trSql.Controls.Add(_cell);
            _tbl.Controls.Add(trSql);
            //trAccess
            trAccess = new HtmlTableRow();
            trAccess.Visible = false;
            //_cell
            _cell = new HtmlTableCell();
            //_literal
            _literal = new Literal();
            _literal.Text = @"
                            مسير فايل Access:
                        ";
            _cell.Controls.Add(_literal);
            trAccess.Controls.Add(_cell);
            //_cell
            _cell = new HtmlTableCell();
            _cell.Attributes["dir"] = @"ltr";
            //uxAccessFileName
            uxAccessFileName = new TextBox();
            uxAccessFileName.Text = @"~/App_Data/ASPNetDB.mdb";
            uxAccessFileName.Width = new Unit("190px");
            _cell.Controls.Add(uxAccessFileName);
            trAccess.Controls.Add(_cell);
            _tbl.Controls.Add(trAccess);
            //_row
            _row = new HtmlTableRow();
            //_cell
            _cell = new HtmlTableCell();
            _cell.Attributes["align"] = @"left";
            _cell.Attributes["style"] = @"height: 49px";
            //uxOK
            uxOK = new Button();
            uxOK.Text = @"انجام تنظيمات";
            uxOK.Click += new System.EventHandler(uxOK_Click);
            _cell.Controls.Add(uxOK);
            _row.Controls.Add(_cell);
            _tbl.Controls.Add(_row);
            view1.Controls.Add(_tbl);
            MultiView1.Views.Add(view1);
            //_ctl1
            View view2 = new View();
            lbl = new Label();
            view2.Controls.Add(lbl);
            //_literal
            _literal = new Literal();
            _literal.Text = @"<br/>مشخصات کاربر Admin:";
            view2.Controls.Add(_literal);
            //uxCreateUser
            uxCreateUser = new Web.UI.CreateUserWizard();
            uxCreateUser.HeaderText = @" ";
            uxCreateUser.UserName = "Admin";
            uxCreateUser.CreatedUser += new System.EventHandler(uxCreateUser_CreatedUser);
            //_ctl3
            CreateUserWizardStep _ctl3 = new CreateUserWizardStep();
            _ctl3.Title = @"";
            uxCreateUser.WizardSteps.Add(_ctl3);
            view2.Controls.Add(uxCreateUser);
            MultiView1.Views.Add(view2);

            View view3 = new View();
            _literal = new Literal();
            _literal.Text = "کاربر Admin با موفقيت ايجاد شد.";
            view3.Controls.Add(_literal);
            MultiView1.Views.Add(view3);

            Form.Controls.Add(MultiView1);
            //uxBack
            uxBack = new Button();
            uxBack.Text = @"بازگشت";
            uxBack.Width = new Unit("60px");
            uxBack.Click += new System.EventHandler(uxBack_Click);
            Form.Controls.Add(uxBack);
        }



        public MultiView MultiView1;
        View view1;
        RadioButtonList uxProviders;
        HtmlTableRow trSql;
        RadioButtonList uxConnectionStrings;
        HtmlTableRow trAccess;
        TextBox uxAccessFileName;
        Button uxOK;
        Unicorn.Web.UI.CreateUserWizard uxCreateUser;
        Button uxBack;
        Label lbl;

        private string backTpPage;
        public string BackTpPage
        {
            get { return backTpPage; }
            set { backTpPage = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            bool hasAccess = false;
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    if (Membership.GetAllUsers().Count == 0)
                        hasAccess = true;
                }
                catch(SqlException)
                {
                    hasAccess = true;
                }
            }
            else
                if (Roles.IsUserInRole("admin"))
                    hasAccess = true;
            if (!hasAccess)
            {
                Response.Write("<h3>Access denied.</h3>");
                Response.End();
                return;
            }

            if (!IsPostBack)
            {
                //System.Configuration.Configuration conf = ConfigUtility.GetConfigFile();

                //uxConnectionStrings
                foreach (ConnectionStringSettings cn in WebConfigurationManager.ConnectionStrings)
                {
                    uxConnectionStrings.Items.Add(new ListItem(cn.Name, cn.Name));
                }
                if (uxConnectionStrings.Items.Count > 1)
                    uxConnectionStrings.SelectedIndex = 1;

                //uxProviders (SelectedIndex)
                //SystemWebSectionGroup sysweb = ConfigUtility.GetSystemWebSectionGroup(conf);
                //string constr = sysweb.Membership.Providers[sysweb.Membership.DefaultProvider].Parameters["connectionStringName"];
            }
        }

        void uxProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            trSql.Visible = (uxProviders.SelectedValue == "sql");
            trAccess.Visible = !trSql.Visible;
        }

        void uxBack_Click(object sender, EventArgs e)
        {
            Exit();
        }

        protected void uxOK_Click(object sender, EventArgs e)
        {
            if (uxProviders.SelectedValue == "sql")
                ConfigInitializer.InitializeSql(uxConnectionStrings.SelectedValue);
            else
                throw new NotSupportedException();
                //ConfigInitializer.InitializeAccess(uxAccessFileName.Text);
            Response.Redirect(UniHttpHandler.HandlerPath + "?a=conf&p=2");
            //MultiView1.ActiveViewIndex = 1;
        }
        protected void uxCreateUser_CreatedUser(object sender, EventArgs e)
        {
            if (!Roles.RoleExists("Admin"))
            {
                Roles.CreateRole("Admin");
                lbl.Text = "نقش کاربري Admin با موفقيت ايجاد شد.";
            }
            Roles.AddUserToRole(uxCreateUser.UserName, "Admin");
            Authorization.AuthorizationManager.AddActionForRole("Admin", "Menu");
            MultiView1.ActiveViewIndex = 2;
        }
        private void Exit()
        {
            if (!string.IsNullOrEmpty(backTpPage))
                Response.Redirect(backTpPage);
            else
                Response.Redirect("~");
        }
    }
}
