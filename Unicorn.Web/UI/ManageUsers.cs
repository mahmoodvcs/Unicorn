using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Profile;
using System.Linq;
using Telerik.Web.UI;

namespace Unicorn.Web.UI
{
    [Bindable(true)]
    [PersistChildren(true)]
    public class ManageUsers : CompositeControl
    {
        public ManageUsers()
        {
            Attributes["dir"] = "rtl";
            //AllowDelete = true;
            //AllowEdit = true;
            //AllowNew = true;
            //ShowCreationDate = true;
            //ShowEmail = true;
            //ShowLastLoginDate = true;
            //ShowRoles = true;
            //AllowEditRoles = true;
            customColumns = new List<CustomColumn>();
        }
        RadGrid uxUsers;
        MultiView multiView;
        EditUserRoles uxEditRoles;

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
        public bool AllowEdit
        {
            get
            {
                object o = ViewState["AllowEdit"];
                if (o != null)
                    return (bool)o;
                return true;
            }
            set { ViewState["AllowEdit"] = value; }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool AllowEditRoles
        {
            get
            {
                object o = ViewState["AllowEditRoles"];
                if (o != null)
                    return (bool)o;
                return true;
            }
            set { ViewState["AllowEditRoles"] = value; }
        }

        [Browsable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public string EditUserPageUrl
        {
            get
            {
                object o = ViewState["EditUserPageUrl"];
                if (o != null)
                    return (string)o;
                return "";
            }
            set { ViewState["EditUserPageUrl"] = value; }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool AllowNew
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


        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowCreationDate
        {
            get
            {
                object o = ViewState["ShowCreationDate"];
                if (o != null)
                    return (bool)o;
                return false;
            }
            set { ViewState["ShowCreationDate"] = value; }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ShowLastLoginDate
        {
            get
            {
                object o = ViewState["ShowLastLoginDate"];
                if (o != null)
                    return (bool)o;
                return true;
            }
            set { ViewState["ShowLastLoginDate"] = value; }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowEmail
        {
            get
            {
                object o = ViewState["ShowEmail"];
                if (o != null)
                    return (bool)o;
                return false;
            }
            set { ViewState["ShowEmail"] = value; }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ShowRoles
        {
            get
            {
                object o = ViewState["ShowRoles"];
                if (o != null)
                    return (bool)o;
                return true;
            }
            set { ViewState["ShowRoles"] = value; }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowPassword
        {
            get
            {
                object o = ViewState["ShowPassword"];
                if (o != null)
                    return (bool)o;
                return false;
            }
            set { ViewState["ShowPassword"] = value; }
        }


        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue("")]
        public string SortExpression
        {
            get { return (string)(ViewState["SortExpression"] ?? ""); }
            set { ViewState["SortExpression"] = value; }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(SortDirection.Ascending)]
        public SortDirection SortDirection
        {
            get { return (SortDirection)(ViewState["SortDirection"] ?? SortDirection.Ascending); }
            set { ViewState["SortDirection"] = value; }
        }

        private List<CustomColumn> customColumns;
        [Browsable(true)]
        [Category("Behavior")]
        [PersistenceModeAttribute(PersistenceMode.InnerProperty)]
        public List<CustomColumn> CustomColumns
        {
            get
            {
                return customColumns;
            }
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
        //[PersistenceModeAttribute(PersistenceMode.InnerProperty)]
        //public virtual DataControlFieldCollection Columns { get; }


        protected override object SaveControlState()
        {
            return new object[] { base.SaveControlState(), customColumns.ToArray() };
        }
        protected override void LoadControlState(object savedState)
        {
            object[] a = (object[])savedState;
            base.LoadControlState(a[0]);
            customColumns = new List<CustomColumn>((CustomColumn[])a[1]);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            multiView = new MultiView();
            uxUsers = new RadGrid();
            //uxUsers.RowStyle.BackColor = Color.FromArgb(0xEF, 0xF3, 0xFB);
            uxUsers.FooterStyle.BackColor = Color.FromArgb(0x50, 0x7C, 0xD1);
            uxUsers.FooterStyle.Font.Bold = true;
            uxUsers.FooterStyle.ForeColor = Color.White;
            uxUsers.HeaderStyle.BackColor = Color.FromArgb(0x50, 0x7C, 0xD1);
            uxUsers.HeaderStyle.Font.Bold = true;
            uxUsers.HeaderStyle.ForeColor = Color.White;
            //uxUsers.AlternatingRowStyle.BackColor = Color.White;
            uxUsers.Skin = "Office2007";
            uxUsers.AutoGenerateColumns = false;
            //uxUsers.EmptyDataTemplate = new EmptyDataTemplate();
            uxUsers.AllowSorting = true;
            uxUsers.AllowFilteringByColumn = true;
            //uxUsers.Sorting += new GridViewSortEventHandler(uxUsers_Sorting);
            GridBoundColumn col = new GridBoundColumn();
            col.DataField = "UserName";
            col.SortExpression = "UserName";
            col.HeaderText = "نام كاربري";
            uxUsers.Columns.Add(col);

            if (ShowEmail)
            {
                col = new GridBoundColumn();
                col.DataField = "Email";
                col.HeaderText = "آدرس ايميل";
                uxUsers.Columns.Add(col);
            }
            if (ShowLastLoginDate)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                //TemplateField tmp = new TemplateField();
                tmp.ItemTemplate = new DateTemplate("LastLoginDate");
                tmp.SortExpression = "LastLoginDate";
                tmp.HeaderText = "تاريخ آخرين ورود";
                uxUsers.Columns.Add(tmp);
            }
            if (ShowCreationDate)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new DateTemplate("CreationDate");
                tmp.SortExpression = "CreationDate";
                tmp.HeaderText = "تاريخ ايجاد حساب كاربري";
                uxUsers.Columns.Add(tmp);
            }
            if (ShowRoles)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new RolesTemplate();
                tmp.HeaderText = "نقشهای کاربر";
                tmp.SortExpression = "Role";
                uxUsers.Columns.Add(tmp);
            }
            if (ShowPassword)
            {
                col = new GridBoundColumn();
                col.DataField = "Password";
                col.HeaderText = "رمز عبور";
                uxUsers.Columns.Add(col);
            }
            //uxUsers.SortExpression

            int i = 0;
            foreach (CustomColumn c in customColumns)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new CustomColumnTemplate(c);
                tmp.HeaderText = c.Caption;
                tmp.SortExpression = "_col," + i.ToString();
                uxUsers.Columns.Add(tmp);
                i++;
            }

            if (AllowEditRoles)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new EditRolesButtonTemplate();
                uxUsers.Columns.Add(tmp);
            }
            if (AllowDelete)
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new DeleteButtonTemplate();
                uxUsers.Columns.Add(tmp);
            }
            if (AllowEdit && !string.IsNullOrEmpty(EditUserPageUrl))
            {
                GridTemplateColumn tmp = new GridTemplateColumn();
                tmp.ItemTemplate = new EditButtonTemplate(EditUserPageUrl);
                uxUsers.Columns.Add(tmp);
            }
            //if (AllowNew)
            //{
            //	TemplateField tmp = new TemplateField();
            //	tmp.ItemTemplate = new DeleteButtonTemplate();
            //	uxUsers.Columns.Add(tmp);
            //}

            //col = new BoundField();
            //col.DataField = "";
            //col.HeaderText = "";
            //uxUsers.Columns.Add( col );

            uxUsers.ItemCommand +=new GridCommandEventHandler(uxUsers_ItemCommand);

            View v = new View();
            v.Controls.Add(uxUsers);
            multiView.Views.Add(v);

            uxEditRoles = new EditUserRoles();
            uxEditRoles.SaveClicked += new EventHandler(uxEditRoles_SaveClicked);
            v = new View();
            v.Controls.Add(uxEditRoles);
            multiView.Views.Add(v);

            multiView.ActiveViewIndex = 0;
            Controls.Add(multiView);
            ChildControlsCreated = true;

        }

        void uxUsers_Sorting(object sender, GridViewSortEventArgs e)
        {
            bool direction = e.SortExpression == SortExpression ? SortDirection == SortDirection.Descending : SortDirection == SortDirection.Ascending;
            BindGrid(e.SortExpression, direction);
            SortExpression = e.SortExpression;
            if (direction)
                SortDirection = SortDirection.Ascending;
            else
                SortDirection = SortDirection.Descending;
        }

        void uxEditRoles_SaveClicked(object sender, EventArgs e)
        {
            BindGrid();
            multiView.ActiveViewIndex = 0;
        }
        //protected override void RenderContents(HtmlTextWriter writer)
        //{
        //	writer.RenderBeginTag("div");
        //	uxUsers.RenderControl(writer);
        //	writer.RenderEndTag();
        //}
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                EnsureChildControls();
                BindGrid();
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Utility.AddStyleSheet(Page, "Unicorn.Web.Resources.control.css");
        }
        private void BindGrid()
        {
            BindGrid(null, false);
        }
        private void BindGrid(string sortExpression, bool ascending)
        {
            //if (DesignMode)
            //{
            //	List<TempMembershipUser> l = new List<TempMembershipUser>();
            //	TempMembershipUser u = new TempMembershipUser();
            //	l.Add(u);
            //	uxUsers.DataSource = l;
            //	uxUsers.
            //}
            //else
            List<MembershipUser> users = new List<MembershipUser>(Membership.GetAllUsers().Cast<MembershipUser>());
            if (sortExpression != null)
            {

                if (sortExpression.StartsWith("_col,"))
                {
                    var c = customColumns[int.Parse(sortExpression.Remove(0, 5))];
                    users.Sort(new Comparison<MembershipUser>((u1, u2) =>
                        ((IComparable)c.GetData(u1.UserName)).CompareTo(c.GetData(u2.UserName))));
                    if (!ascending)
                        users.Reverse();
                }
                else
                {
                    DataControlField col = null;
                    foreach (DataControlField c in uxUsers.Columns)
                    {
                        if (c.SortExpression == sortExpression)
                            col = c;
                    }
                    if (col != null)
                    {
                        if (sortExpression == "Role")
                        {
                            users.Sort((u1, u2) => GetUserRole(u1.UserName).CompareTo(GetUserRole(u2.UserName)) * (ascending ? 1 : -1));
                        }
                        else
                        {
                            PropertyInfo pi = typeof(MembershipUser).GetProperty(sortExpression);
                            users.Sort(new Comparison<MembershipUser>((u1, u2) => ((IComparable)pi.GetValue(u1, null)).CompareTo(pi.GetValue(u2, null)) * (ascending ? 1 : -1)));
                        }
                    }
                }
            }
            uxUsers.DataSource = users;
            uxUsers.DataBind();
        }

        private string GetUserRole(string userName)
        {
            string[] roles = Roles.GetRolesForUser(userName);
            if (roles.Length > 0)
                return roles[0];
            else
                return "";
        }
        void uxUsers_ItemCommand(object source, GridCommandEventArgs e)
        //void uxUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "del":
                    Membership.DeleteUser((string)e.CommandArgument);
                    BindGrid();
                    break;
                case "edit":
                    break;
                case "editRoles":
                    uxEditRoles.UserName = (string)e.CommandArgument;
                    multiView.ActiveViewIndex = 1;
                    break;
            }
        }

        #region Field Templates

        private class EmptyDataTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Label l = new Label();
                l.Text = "هیچ کاربری ثبت نشده است";
                container.Controls.Add(l);
            }

        }

        private class DeleteButtonTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Button btn = new Button();
                btn.CommandName = "del";
                btn.DataBinding += new EventHandler(btn_DataBinding);
                btn.Text = "حذف كاربر";
                btn.OnClientClick = "if( !confirm('آيا مطمئن هستيد؟') ) return false;";
                container.Controls.Add(btn);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                Button b = (Button)sender;
                GridDataItem row = (GridDataItem)b.NamingContainer;
                b.CommandArgument = DataBinder.Eval(row.DataItem, "UserName").ToString();
            }
        }

        private class EditButtonTemplate : ITemplate
        {
            public EditButtonTemplate(string url)
            {
                this.url = url;
            }
            string url;

            public void InstantiateIn(Control container)
            {
                HyperLink lnk = new HyperLink();
                lnk.Text = "ویرایش کاربر";
                //Button btn = new Button();
                //btn.CommandName = "edit";
                lnk.DataBinding += new EventHandler(btn_DataBinding);
                //btn.Text = "ویرایش كاربر";
                container.Controls.Add(lnk);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                HyperLink lnk = (HyperLink)sender;
                GridDataItem row = (GridDataItem)lnk.NamingContainer;
                //GridViewRow row = (GridViewRow)lnk.NamingContainer;
                lnk.NavigateUrl = string.Format(url, DataBinder.Eval(row.DataItem, "UserName").ToString());
            }
        }

        private class EditRolesButtonTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Button btn = new Button();
                btn.CommandName = "editRoles";
                btn.DataBinding += new EventHandler(btn_DataBinding);
                btn.Text = "ويرايش نقشها";
                container.Controls.Add(btn);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                Button b = (Button)sender;
                GridDataItem row = (GridDataItem)b.NamingContainer;
                b.CommandArgument = DataBinder.Eval(row.DataItem, "UserName").ToString();
            }
        }

        private class RolesTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Label l = new Label();
                l.DataBinding += new EventHandler(btn_DataBinding);
                container.Controls.Add(l);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                Label l = (Label)sender;
                GridDataItem row = (GridDataItem)l.NamingContainer;
                string un = DataBinder.Eval(row.DataItem, "UserName").ToString();
                l.Text = string.Join(", ", Roles.GetRolesForUser(un));
            }
        }
        #endregion

        public class CustomColumnTemplate : ITemplate
        {
            public CustomColumnTemplate(CustomColumn col)
            {
                this.col = col;
            }
            CustomColumn col;

            public void InstantiateIn(Control container)
            {
                if (col.ContentType.IsInstanceOfType(new Control()))
                {

                }
                Label l = new Label();
                l.DataBinding += new EventHandler(btn_DataBinding);
                container.Controls.Add(l);
            }

            void btn_DataBinding(object sender, EventArgs e)
            {
                Label l = (Label)sender;
                GridDataItem row = (GridDataItem)l.NamingContainer;
                l.Text = col.GetData(DataBinder.Eval(row.DataItem, "UserName").ToString()).ToString();
            }

        }
    }

    [Serializable]
    public abstract class CustomColumn
    {
        public CustomColumn()
        {

        }
        private string caption;
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
            }
        }

        public abstract object GetData(string userName);
        public virtual Type ContentType
        {
            get { return typeof(string); }
        }
        //public abstract int CompareUsers(MembershipUser user1, MembershipUser user2);
    }

    public class ProfileColumn : CustomColumn
    {
        public ProfileColumn()
        {
        }
        private string propertyName;
        public string PropertyName
        {
            get { return propertyName; }
            set
            {
                propertyName = value;
            }
        }

        public override object GetData(string userName)
        {
            return System.Web.Profile.ProfileBase.Create(userName).GetPropertyValue(propertyName) ?? "";
        }

        //public override int CompareUsers(MembershipUser user1, MembershipUser user2)
        //{
        //    return ((IComparable)GetData(user1.UserName).GetPropertyValue(propertyName))
        //        .CompareTo(ProfileBase.Create(user2.UserName).GetPropertyValue(propertyName));
        //}
    }

    public class ProfileLookupColumn : ProfileColumn
    {
        public ProfileLookupColumn()
        {
        }
        public ProfileLookupColumn(string tableName, string keyColumnName, string textColumnName)
        {
            this.tableName = tableName;
            this.keyColumnName = keyColumnName;
            this.textColumnName = textColumnName;
        }

        private string tableName;
        public string TableName
        {
            get { return tableName; }
            set
            {
                tableName = value;
            }
        }
        private string keyColumnName;
        public string KeyColumnName
        {
            get { return keyColumnName; }
            set
            {
                keyColumnName = value;
            }
        }
        private string textColumnName;
        public string TextColumnName
        {
            get { return textColumnName; }
            set
            {
                textColumnName = value;
            }
        }
        public override object GetData(string userName)
        {
            object p = base.GetData(userName);
            object o;
            if (p.GetType().IsArray)
            {
                var dr = Unicorn.Data.SqlHelper.ExecuteReader(
                    string.Format("select {1} from {2} where {0} in ({3})", keyColumnName, textColumnName, tableName,
                        string.Join(",", ((Array)p).Cast<object>().Select(o1 => o1.ToString()).ToArray())));
                string s = "";
                bool b = true;
                while (dr.Read())
                {
                    if (b) b = false;
                    else s += ",";
                    s += dr[0].ToString();
                }
                o = s;
            }
            else
                o = Unicorn.Data.SqlHelper.ExecuteScaler(
                    string.Format("select {1} from {2} where {0}=@p", keyColumnName, textColumnName, tableName),
                    new SqlParameter("@p", p));
            if (o == null || o == DBNull.Value)
                return "";
            return o;
        }

        //public override int CompareUsers(MembershipUser user1, MembershipUser user2)
        //{
        //    return ((IComparable)GetData(user1.UserName)).CompareTo(GetData(user2.UserName));
        //}
    }
}
