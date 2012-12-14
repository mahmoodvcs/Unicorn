using System;
using System.Collections.Generic;

using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using Unicorn.Data;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Unicorn.Web.Security.Authorization;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:UniDataGrid runat=server></{0}:UniDataGrid>")]
    [PersistChildren(true)]
    public class UniDataGrid : Telerik.Web.UI.RadGrid//CompositeControl
    {
        public UniDataGrid()
        {
            uxGrid = this; //new ComponentArt.Web.UI.DataGrid();
            //uxGrid.Levels.Add(new GridLevel());
            uxGrid.ID = "uxGrid";
            uxGrid.AllowAutomaticUpdates = AllowEditing;
            uxGrid.ClientSettings.EnablePostBackOnRowClick = true;
            uxGrid.Skin = "Office2007";
            uxGrid.AllowPaging = true;
            uxGrid.AllowFilteringByColumn = true;
            uxGrid.PageSize = 10;
            uxGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(uxGrid_NeedDataSource);
            uxGrid.UpdateCommand += new Telerik.Web.UI.GridCommandEventHandler(uxGrid_UpdateCommand);
            uxGrid.DeleteCommand += new Telerik.Web.UI.GridCommandEventHandler(uxGrid_DeleteCommand);
            uxGrid.InsertCommand += new Telerik.Web.UI.GridCommandEventHandler(uxGrid_InsertCommand);
            uxGrid.ItemCommand += new GridCommandEventHandler(uxGrid_ItemCommand);

            //Controls.Add(uxGrid);
        }

        void uxGrid_ItemCommand(object source, GridCommandEventArgs e)
        {

        }
        Telerik.Web.UI.RadGrid uxGrid;

        #region Properties

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string TableName
        {
            get
            {
                return (string)(TableNameInternal ?? (TableInfo == null ? "" : TableInfo.TableName));
            }
            set
            {
                if (TableNameInternal != null && value == TableNameInternal)
                    return;
                TableNameInternal = value;
                if (TableInfo != null && TableInfo.TableName == value)
                    return;
                TableInfoInternal = null;
            }
        }
        string TableNameInternal
        {
            get { return (string)(ViewState["TableName"] ?? null); }
            set { ViewState["TableName"] = value; }

        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string KeyFieldName
        {
            get
            {
                object o = ViewState["KeyFieldName"];
                if (o != null)
                    return (string)o;
                if (TableInfo != null)
                    foreach (var c in TableInfo.Columns)
                    {
                        if (c.IsPrimaryKey)
                            return c.ColumnName;
                    }
                return null;
            }
            set { ViewState["KeyFieldName"] = value; }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(false)]
        public bool AllowEditing
        {
            get
            {
                return (bool)(ViewState["editing"] ?? false);
            }
            set { ViewState["editing"] = value; }
        }
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(false)]
        public bool AllowDeleting
        {
            get
            {
                return (bool)(ViewState["deleting"] ?? false);
            }
            set { ViewState["deleting"] = value; }
        }
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool EnableClientSideEditing
        {
            get
            {
                return (bool)(ViewState["clEd"] ?? true);
            }
            set { ViewState["clEd"] = value; }
        }

        public bool EnableFilteringByColumn
        {
            get
            {
                return (bool)(ViewState["colFilter"] ?? true);
            }
            set { ViewState["colFilter"] = value; }
        }
        public bool ShowSelectCheckBox
        {
            get
            {
                return (bool)(ViewState["shChk"] ?? true);
            }
            set { ViewState["shChk"] = value; }
        }


        public TableInfo TableInfo
        {
            get
            {
                if (TableInfoInternal == null)
                    Initialize();
                return TableInfoInternal;
            }
            set { TableInfoInternal = value; }
        }
        private TableInfo TableInfoInternal
        {
            get
            {
                return (TableInfo)(ViewState["TableInfo"] ?? null);
            }
            set { ViewState["TableInfo"] = value; }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.LoadComplete += new EventHandler(Page_LoadComplete);
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            EnsureChildControls();
            //  CreateGridColumns();

        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(TableNameInternal))
                return;
            if (TableInfoInternal == null || TableInfoInternal.TableName != TableName)
                TableInfoInternal = Data.SchemaUtility.GetTable(TableName);
        }

        private void CreateGridColumns()
        {

            if (TableInfo == null)
                return;
            TableInfo ti = TableInfo;
            int i = 0;
            uxGrid.MasterTableView.Columns.Clear();
            foreach (ColumnInfo c in ti.Columns)
            {
                if (c.IsPrimaryKey)
                {
                    uxGrid.MasterTableView.DataKeyNames = new string[] { c.ColumnName };
                    if (c.IsIdentity)
                        continue;
                }
                GridEditableColumn col;
                var fk = c.ForeignKeyRelation;
                if (fk == null)
                {
                    if (c.DataType == DbType.Boolean)
                    {
                        col = new GridCheckBoxColumn();

                        uxGrid.MasterTableView.Columns.Insert(i, col);
                        ((GridCheckBoxColumn)col).DataField = c.ColumnName;
                    }
                    else
                    {
                        col = new GridBoundColumn();

                        uxGrid.MasterTableView.Columns.Insert(i, col);
                        ((GridBoundColumn)col).DataField = c.ColumnName;

                    }
                    col.DataType = DbTypeUtility.GetType(c.DataType);
                }
                else
                {

                    GridDropDownColumn dc = new GridDropDownColumn();
                    col = dc;
                    if (c.ColumnName.ToLower() != "unionid")
                    {
                        uxGrid.MasterTableView.Columns.Insert(i, col);
                        dc.DataField = c.ColumnName;
                        dc.DataType = DbTypeUtility.GetType(c.DataType);
                        dc.DataSourceID = "gidDS_" + c.ColumnName;
                        dc.ListTextField = "_col_";
                        dc.ListValueField = fk.PKField;
                    }
                }
                col.HeaderText = c.Title;
                col.SortExpression = c.ColumnName;
                col.ShowFilterIcon = false;
                col.CurrentFilterFunction = GridKnownFunction.Contains;
                col.AutoPostBackOnFilter = true;
                //col.UniqueName = c.ColumnName;
                i++;
            }

        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        #region Grid methods
        void uxGrid_InsertCommand(object sender, GridCommandEventArgs e)
        {
            if (!AllowEditing)
                return;
            if (TableInfo == null)
                return;
            Record r = GetRecord(e.Item);
            r.Insert();
        }
        void uxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            //uxGrid.SelectedValue;

        }

        void uxGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (!AllowDeleting)
                return;
            if (TableInfo == null)
                return;
            Record r = GetRecord(e.Item);
            r.Delete();
        }

        void uxGrid_UpdateCommand(object sender, GridCommandEventArgs e)
        {

            if (!AllowEditing)
                return;
            if (TableInfo == null)
                return;
            Record r = GetRecord(e.Item);
            r.Update();
        }

        private Record GetRecord(GridItem item)
        {
            Record r = new Record(TableName);
            //foreach (var ci in TableInfo.Columns)
            //{
            //    if (ci.IsPrimaryKey)
            //        r.Keys[ci.ColumnName] = item[ci.ColumnName];
            //    if (!ci.IsIdentity)
            //        r[ci.ColumnName] = item[ci.ColumnName];
            //}
            return r;
        }
        void uxGrid_NeedDataSource(object sender, EventArgs e)
        {
            if (TableInfo == null)
                return;
            DataTable dt;
            dt = SqlHelper.ExecuteCommand(GetViewSelectCommand());

            uxGrid.DataSource = dt;
        }


        #endregion Grid methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }



        private string GetViewSelectCommand()
        {
            List<string> columns = new List<string>();
            //System.Data.Common.DbConnection con = ConnectionManager.Connection;
            foreach (ColumnInfo ci in TableInfo.Columns)
            {
                if (ci.IsPrimaryKey)
                {
                    uxGrid.MasterTableView.DataKeyNames = new string[] { ci.Title != "" ? ci.Title : ci.ColumnName };
                }
                ForeignKeyRelation fk = ci.ForeignKeyRelation;
                if (fk != null)
                {
                    TableInfo ti = SchemaUtility.GetTable(fk.PKTable);
                    string nameColumn = ti.GetNameColumn().ColumnName;
                    columns.Add("( Select  [" + nameColumn + "] From " + fk.PKTable + " Where " + fk.PKTable + ".[" + fk.PKField + "] =  " + TableName + ".[" + fk.FKField + "]  ) ");
                    break;
                }
                else
                    columns.Add(ci.ColumnName + " as [" + ci.Title + "]");//+ " as [" + ci.Title + "]");
            }
            return "select " + string.Join(",", columns.ToArray()) + " from " + TableName;
        }
    }
}
