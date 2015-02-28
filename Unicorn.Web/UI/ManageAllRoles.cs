using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using Unicorn.Web.Security;

namespace Unicorn.Web.UI
{
	[Bindable(true)]
	public class ManageAllRoles : CompositeControl
	{
		public ManageAllRoles()
		{
			//Attributes["dir"] = "rtl";
		}
		GridView uxRoles;
		TextBox uxNewRoleName, uxNewRoleTitle;
		Button uxAddRole;

		#region Properties

		[Browsable(true)]
		[DefaultValue(true)]
		[Category("Behavior")]
		public bool AllowDelete
		{
			get
			{
				object o = ViewState["AllowDelete"];
				if (o != null)
					return (bool)o;
				return true;
			}
			set { ViewState["AllowDelete"] = value; }
		}

        [Browsable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public string EditAccessesPageUrl
        {
            get
            {
                object o = ViewState["EditAccessesPageUrl"];
                if (o != null)
                    return (string)o;
                return "";
            }
            set { ViewState["EditAccessesPageUrl"] = value; }
        }

		//[Browsable( true )]
		//[DefaultValue( true )]
		//[Category( "Behavior" )]
		//public bool s
		//{
		//	get
		//	{
		//		object o = ViewState[""];
		//		if ( o != null )
		//			return (bool)o;
		//		return true;
		//	}
		//	set { ViewState[""] = value; }
		//}

		#endregion Properties

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			uxNewRoleName = new TextBox();
			Controls.Add(uxNewRoleName);

            uxNewRoleTitle = new TextBox();
            Controls.Add(uxNewRoleTitle);

			uxAddRole = new Button();
			uxAddRole.Text = "اضافه";
			uxAddRole.Click += new EventHandler(uxAddRole_Click);
			Controls.Add(uxAddRole);

			uxRoles = new GridView();
			uxRoles.HeaderStyle.BackColor = Color.Gray;
			uxRoles.HeaderStyle.Font.Bold = true;
			uxRoles.AlternatingRowStyle.BackColor = Color.LightGreen;
			uxRoles.AutoGenerateColumns = true;
            uxRoles.DataKeyNames = new string[]{ "RoleName" };
			uxRoles.PageSize = 10;

			//BoundField col = new BoundField();
			//col.
			////col.DataField = "RoleName";
			//col.HeaderText = "نام نقش كاربري";
			//uxRoles.Columns.Add( col );

            //if (AllowManage)
            //{
            //    TemplateField tmp = new TemplateField();
            //    tmp.ItemTemplate = new ManageButtonTemplate();
            //    uxRoles.Columns.Add(tmp);
            //}
            if (!string.IsNullOrEmpty(EditAccessesPageUrl))
            {
                TemplateField tmp = new TemplateField();
                //TemplateColumn tmp = new TemplateColumn();
                tmp.ItemTemplate = new EditAccessesButtonTemplate(EditAccessesPageUrl, "ویرایش دسنرسی ها");
                uxRoles.Columns.Add(tmp);
            }
			if (AllowDelete)
			{
				TemplateField tmp = new TemplateField();
				tmp.ItemTemplate = new DeletButtonTemplate();
				uxRoles.Columns.Add(tmp);
			}

			uxRoles.RowCommand += new GridViewCommandEventHandler(uxRoles_RowCommand);
			Controls.Add(uxRoles);
			ChildControlsCreated = true;
		}

		void uxAddRole_Click(object sender, EventArgs e)
		{
			if (uxNewRoleName.Text.Trim() != "" && !Roles.RoleExists(uxNewRoleName.Text))
			{
                string title = uxNewRoleTitle.Text;
                if (title.Trim() == "")
                    title = uxNewRoleName.Text;
				UniRoles.CreateRole(uxNewRoleName.Text,title );
				BindGrid();
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Table;
			}
		}
		protected override void RenderContents(HtmlTextWriter writer)
		{
            writer.AddStyleAttribute("direction", "rtl");
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);

			writer.Write("<h4>نقش کاربري جديد</h4><br>");
            writer.Write("نام: ");
            uxNewRoleName.RenderControl(writer);
            writer.Write("<br/>عنوان فارسي: ");
            uxNewRoleTitle.RenderControl(writer);
            uxAddRole.RenderControl(writer);

			writer.RenderEndTag();
			writer.RenderEndTag();

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);

			uxRoles.RenderControl(writer);

			writer.RenderEndTag();
			writer.RenderEndTag();
			writer.RenderEndTag();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Page.IsPostBack)
			{
				EnsureChildControls();
				BindGrid();
			}
		}
		protected override void OnPreRender( EventArgs e )
		{
			base.OnPreRender( e );
			Utility.AddStyleSheet(Page, "Jco.Security.Resources.control.css");
		}
		private void BindGrid()
		{
			if (System.Web.Security.Roles.Enabled)
			{
				var roles = UniRoles.GetAllRoles();
				uxRoles.DataSource = roles;

				int currentPage = uxRoles.PageIndex;
				int count = roles.Length;
				int pageSize = uxRoles.PageSize;
				if (count > 0 && currentPage == count / pageSize && count % pageSize == 0)
				{
					uxRoles.PageIndex -= 1;
				}
				uxRoles.DataBind();
			}
			else
			{
				uxRoles.Visible = false;
				//Unicorn.Web.Security.Configuration.ConfigInitializer.CheckConfig(this);
			}
			//uxRoles.Visible = (uxRoles.Rows.Count > 0);
		}


		void uxRoles_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "del":
                    if (System.Web.Security.Roles.FindUsersInRole((string)e.CommandArgument, "%").Length > 0)
					{
						Unicorn.Web.WebUtility.ShowMessageBox("خطا: اين نقش نمي تواند حذف شود زيرا تعدادي کاربر در اين نقش وجود دارد", Page);
					}
					else
					{
                        System.Web.Security.Roles.DeleteRole((string)e.CommandArgument);
						BindGrid();
					}
					break;
			}
		}

		#region Button Templates

		private class DeletButtonTemplate : ITemplate
		{
			public void InstantiateIn(Control container)
			{
				Button btn = new Button();
				btn.CommandName = "del";
				btn.DataBinding += new EventHandler(btn_DataBinding);
				btn.Text = "حذف";
				btn.OnClientClick = "if( !confirm('آيا مطمئن هستيد؟') ) return false;";
				container.Controls.Add(btn);
			}

			void btn_DataBinding(object sender, EventArgs e)
			{
				Button b = (Button)sender;
				GridViewRow row = (GridViewRow)b.NamingContainer;
				b.CommandArgument = ((UniRole)row.DataItem).RoleName;
			}
		}

		private class ManageButtonTemplate : ITemplate
		{
			public void InstantiateIn(Control container)
			{
				Button btn = new Button();
				btn.CommandName = "manage";
				btn.DataBinding += new EventHandler(btn_DataBinding);
				btn.Text = "كاربران";
				container.Controls.Add(btn);
			}

			void btn_DataBinding(object sender, EventArgs e)
			{
				Button b = (Button)sender;
				GridViewRow row = (GridViewRow)b.NamingContainer;
                b.CommandArgument = ((UniRole)row.DataItem).RoleName;
			}
		}
        public class EditAccessesButtonTemplate : ITemplate
        {
            public EditAccessesButtonTemplate(string url, string title = null)
            {
                this.url = url;
                this.title = title;
            }
            string url, title;

            public void InstantiateIn(Control container)
            {
                HyperLink lnk = new HyperLink();
                lnk.Text = title ?? "ويرايش کاربر";
                //Button btn = new Button();
                //btn.CommandName = "edit";
                lnk.DataBinding += new EventHandler(btn_DataBinding);
                //btn.Text = "ويرايش كاربر";
                container.Controls.Add(lnk);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                HyperLink lnk = (HyperLink)sender;
                GridViewRow row = (GridViewRow)lnk.NamingContainer;
                lnk.NavigateUrl = string.Format(url, ((UniRole)row.DataItem).RoleName);
            }
        }
		#endregion

	}
}
