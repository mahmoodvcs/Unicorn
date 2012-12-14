using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Unicorn.Web.Security;
using System.Security.Permissions;

namespace Unicorn.Web.UI
{
	//[Designer( "System.Web.UI.Design.WebControls.LoginDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" ),
	//Bindable( false )]
	public class AdminMenu : Menu
	{
		public AdminMenu()
		{
		}
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if ( ShowCreateUserItem && FindItem( "CreateUser" ) == null )
			{
				MenuItem m = new MenuItem();
				m.Text = "ايجاد كاربر جديد";
				m.Value = "CreateUser";
				m.NavigateUrl = UniHttpHandler.HandlerPath + "?a=cu";
				Items.Add( m );
			}
		}

		#region Properties

		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		public bool ShowCreateUserItem
		{
			get
			{
				object o = ViewState["ShowCreateUserItem"];
				if ( o != null )
					return (bool)o;
				return true;
			}
			set { ViewState["ShowCreateUserItem"] = value; }
		}
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]

		public bool ShowManageUsersItem
		{
			get
			{
				object o = ViewState["ShowManageUsersItem"];
				if ( o != null )
					return (bool)o;
				return true;
			}
			set { ViewState["ShowManageUsersItem"] = value; }
		}

		#endregion Properties


		//[BrowsableAttribute( false )]
		//[PersistenceModeAttribute( PersistenceMode.InnerProperty )]
		//[TemplateContainerAttribute( typeof( AdminMenu ) )]
		//public virtual ITemplate LayoutTemplate
		//{
		//	get;
		//	set;
		//}
		//protected override void CreateChildControls()
		//{
		//	this.Controls.Clear();
		//	if ( LayoutTemplate == null )
		//		LayoutTemplate = new AdminMenuTemplate();
		//	Control c = new Control();
		//	LayoutTemplate.InstantiateIn( c );
		//	Controls.Add( c );
		//	ChildControlsCreated = true;
		//}
	}

	public class AdminMenuTemplate : ITemplate
	{
		public void InstantiateIn( Control container )
		{
			Menu menu = new Menu();
			MenuItem m = new MenuItem();
			m.Text = "ايجاد كاربر جديد";
			m.NavigateUrl = UniHttpHandler.HandlerPath + "?a=cu";
			menu.Items.Add( m );
			container.Controls.Add( menu );
		}
	}
}
