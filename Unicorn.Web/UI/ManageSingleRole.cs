using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

namespace Unicorn.Web.UI
{
	[Bindable( true )]
	public class ManageSingleRole: CompositeControl
	{
		public ManageSingleRole()
		{
			Attributes["dir"] = "rtl";
		}
		GridView uxRoles;
	
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
		[DefaultValue(true)]
		[Category("Behavior")]
		public bool AllowManage
		{
			get
			{
				object o = ViewState["AllowNew"];
				if (o != null)
					return (bool)o;
				return true;
			}
			set { ViewState["AllowNew"] = value; }
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
			uxRoles = new GridView();
			uxRoles.HeaderStyle.BackColor = Color.Gray;
			uxRoles.HeaderStyle.Font.Bold = true;
			uxRoles.AlternatingRowStyle.BackColor = Color.LightGreen;
			uxRoles.AutoGenerateColumns = false;
			uxRoles.PageSize = 10;

			BoundField col = new BoundField();
			//col.DataField = "RoleName";
			col.HeaderText = "نام نقش كاربري";
			uxRoles.Columns.Add( col );

			if (AllowManage)
			{
				TemplateField tmp = new TemplateField();
				tmp.ItemTemplate = new ManageButtonTemplate();
				uxRoles.Columns.Add(tmp);
			}
			if (AllowDelete)
			{
				TemplateField tmp = new TemplateField();
				tmp.ItemTemplate = new DeletButtonTemplate();
				uxRoles.Columns.Add(tmp);
			}

			uxRoles.RowCommand += new GridViewCommandEventHandler( uxRoles_RowCommand );
			Controls.Add( uxRoles );
			ChildControlsCreated = true;
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
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);

		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Page.IsPostBack)
			{
				BindGrid();
			}
		}
		//protected override void OnPreRender( EventArgs e )
		//{
		//	try
		//	{
		//		uxRoles.DataSource = Roles.GetAllRoles();
		//		DataBind();
		//	}
		//	catch (Exception ex)
		//	{
		//		uxRoles.EmptyDataText = "Error: " + ex.Message;
		//	}
		//	base.OnPreRender( e );
		//}
		private void BindGrid()
		{
			string[] arr = Roles.GetAllRoles();
			uxRoles.DataSource = arr;

			int currentPage = uxRoles.PageIndex;
			int count = arr.Length;
			int pageSize = uxRoles.PageSize;
			if (count > 0 && currentPage == count / pageSize && count % pageSize == 0)
			{
				uxRoles.PageIndex -= 1;
			}
			uxRoles.DataBind();
			//uxRoles.Visible = (uxRoles.Rows.Count > 0);
		}


		void uxRoles_RowCommand( object sender, GridViewCommandEventArgs e )
		{
			switch ( e.CommandName )
			{
				case "del":
					Membership.DeleteUser( (string)e.CommandArgument );
					break;
				case "edit":
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
				b.CommandArgument = DataBinder.Eval(row.DataItem, "").ToString();
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
				b.CommandArgument = DataBinder.Eval(row.DataItem, "").ToString();
			}
		}
		#endregion

	}
}
