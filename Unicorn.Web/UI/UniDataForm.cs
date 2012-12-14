using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using Unicorn.Data;
using System.Globalization;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Data.Common;
using System.Data.SqlClient;
using Telerik.Web.UI;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:UniDataForm runat=server></{0}:UniDataForm>")]
    public class UniDataForm : CompositeControl
    {
        public UniDataForm()
        {
            uxToolbar = new UniDataToolbar();
            uxToolbar.SearchClicked += new EventHandler(uxToolbar_SearchClicked);
            Attributes["dir"] = "rtl";
        }

        void uxToolbar_SearchClicked(object sender, EventArgs e)
        {
            if (Mode == DataFormMode.SingleRecord)

                Mode = DataFormMode.List;
            else
                Mode = DataFormMode.SingleRecord;
        }
        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            base.OnLoad(e);
            //Page.Trace.Write("JcoDataForm.OnLoad()");
            Initialize();

            if (Condition != null && Condition != "")
            {
                uxToolbar.SearchForm();
                Condition = null;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //EnsureChildControls();
        }



        private UniDataToolbar uxToolbar;
        private Panel panel;
        List<Control> dataControls;
        //List<string> fieldGroup;
        private List<DataColumn> columns;
        //private List<FieldOption> fieldOption;
        private UniDataGrid uxGrid;

        #region Properties

        //[Category("Behavior")]
        //[Localizable(false)]
        //[DefaultValue(true)]
        //public bool ShowToolbar { get; set; }

        private string tableName;
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string TableName
        {
            get
            {
                EnsureChildControls();
                return uxToolbar.TableName;
            }
            set
            {
                if (ChildControlsCreated)
                {
                    if (uxToolbar.TableName == value)
                        return;
                    uxToolbar.TableName = value;
                    IsInitialized = false;
                    Initialize();
                }
                else
                    tableName = value;
            }
        }

        private string keyFieldName;
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string KeyFieldName
        {
            get
            {
                EnsureChildControls();
                return uxToolbar.KeyFieldName;
            }
            set
            {
                if (ChildControlsCreated)
                    uxToolbar.KeyFieldName = value;
                else
                    keyFieldName = value;
            }
        }

        public object CurrentKeyValue
        {
            get
            {
                return uxToolbar.CurrentKeyValue;
            }

        }

        private string condition;
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string Condition
        {
            get
            {
                EnsureChildControls();
                return uxToolbar.Condition;
            }
            set
            {
                if (ChildControlsCreated)
                    uxToolbar.Condition = value;
                else
                    condition = value;
            }
        }

        string sortExpression;
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string SortExpression
        {
            get
            {
                EnsureChildControls();
                return uxToolbar.SortExpression;
            }
            set
            {
                if (ChildControlsCreated)
                    uxToolbar.SortExpression = value;
                else
                    sortExpression = value;
            }
        }

        private string filterExpression;
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string FilterExpression
        {
            get
            {
                EnsureChildControls();
                return uxToolbar.FilterExpression;
            }
            set
            {
                if (ChildControlsCreated)
                    uxToolbar.FilterExpression = value;
                else
                    filterExpression = value;
            }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool AutoGenerateColumns
        {
            get { return (bool)(ViewState["autogen"] ?? true); }
            set { ViewState["autogen"] = value; }
        }

        public TableInfo TableInfo
        {
            get { return uxToolbar.TableInfo; }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(DataFormMode.SingleRecord)]
        public DataFormMode DefaultMode
        {
            get { return (DataFormMode)(ViewState["DefaultMode"] ?? DataFormMode.SingleRecord); }
            set { ViewState["DefaultMode"] = value; }
        }
        public Record DataRecord
        {
            get { return uxToolbar.DataRecord; }
        }

        public string ExcludeColumns
        {
            get { return (string)(ViewState["ExcludeColumns"] ?? ""); }
            set { ViewState["ExcludeColumns"] = value; }
        }
        //public int ColumnShowCount { get; set; }
        public DataFormMode Mode
        {
            get { return (DataFormMode)(ViewState["Mode"] ?? DefaultMode); }
            set { ViewState["Mode"] = value; }
        }
        public bool IsInitialized
        {
            get { return (bool)(ViewState["IsInitialized"] ?? false); }
            private set { ViewState["IsInitialized"] = value; }
        }
        public bool ShowDataToolBar
        {
            get { return (bool)(ViewState["ShowDataToolBar"] ?? true); }
            private set { ViewState["ShowDataToolBar"] = value; }
        }
        #endregion Properties

        #region Events
        public event EventHandler CurrentRowChanged
        {
            add
            {
                uxToolbar.CurrentRowChanged += value;
            }
            remove
            {
                uxToolbar.CurrentRowChanged -= value;

            }
        }

        public event EventHandler PrintClick
        {
            add
            {
                uxToolbar.PrintClicked += value;
            }
            remove
            {
                uxToolbar.PrintClicked -= value;

            }
        }


        public event CancelEventHandler RowDeleting
        {
            add
            {
                uxToolbar.RowDeleting += value;
            }
            remove
            {
                uxToolbar.RowDeleting -= value;

            }
        }

        public event EventHandler RowUpdated
        {
            add
            {
                uxToolbar.RowUpdated += value;
            }
            remove
            {
                uxToolbar.RowUpdated -= value;

            }
        }

        public event EventHandler RowInserted
        {
            add
            {
                uxToolbar.RowInserted += value;
            }
            remove
            {
                uxToolbar.RowInserted -= value;

            }
        }

        public event EventHandler RowDeleted
        {
            add
            {
                uxToolbar.RowDeleted += value;
            }
            remove
            {
                uxToolbar.RowDeleted -= value;

            }
        }

        public event CancelEventHandler RowUpdating
        {
            add
            {
                uxToolbar.RowUpdating += value;
            }
            remove
            {
                uxToolbar.RowUpdating -= value;

            }
        }

        public event CancelEventHandler RowInserting
        {
            add
            {
                uxToolbar.RowInserting += value;
            }
            remove
            {
                uxToolbar.RowInserting -= value;

            }
        }


        #endregion

        private void Initialize()
        {
            if (Page.Trace.IsEnabled)
                Page.Trace.Write("JcoDataForm.Initialize()");
            if (IsInitialized)
            {
                CreateColumns();
                CreateDataControls();
                return;
            }
            uxToolbar.TableInfo.LoadDescriptions();
            uxToolbar.TableInfo.ForeignKeys.Load();
            CreateColumns();
            CreateDataControls();
            IsInitialized = true;
        }

        #region Create Columns & Data Controls

        private void CreateColumns()
        {
            if (TableInfo == null)
                return;
            string[] exclude = ExcludeColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim().ToLower()).ToArray();
            columns = new List<DataColumn>();
            if (AutoGenerateColumns)
                foreach (var c in TableInfo.Columns)
                {
                    if (Array.IndexOf(exclude, c.ColumnName.ToLower()) >= 0)
                        continue;
                    //if (!c.dontShow)
                    //{
                    AutomaticDataColumn col = new AutomaticDataColumn();
                    col.DataField = c.ColumnName;
                    columns.Add(col);
                    //}

                }
        }

        private void CreateDataControls()
        {
            if (TableInfo == null)
                return;
            //if (dataControls != null)
            //    return;

            if (Mode == DataFormMode.SingleRecord)
            {
                dataControls = new List<Control>();
                //fieldOption = new List<FieldOption>();
                //fieldGroup = new List<string>();
                Control contol;// = new Control();
                foreach (var c in columns)
                {
                    ColumnInfo ci = TableInfo.Columns[c.DataField];
                    ForeignKeyRelation fk = ci.ForeignKeyRelation;

                    //string panel = ci.panelName == null ? "" : ci.panelName;
                    //if (!fieldGroup.Contains(panel))
                    //    fieldGroup.Add(panel);

                    if (c is AutomaticDataColumn)
                    {
                        //FieldOption field = new FieldOption();
                        //field.column = TableInfo.Columns[c.DataField];
                        if (fk == null)
                        {
                            contol = CreataNormalDataControl(c);
                            dataControls.Add(contol);
                        }
                        else
                        {
                            contol = CreataForeignKeyDataControl(c, fk);
                            dataControls.Add(contol);
                        }
                    }
                    //TODO: Not autogenerate
                }
                foreach (var c in dataControls)
                {
                    panel.Controls.Add(c);
                }
            }

        }

        private Control CreataForeignKeyDataControl(DataColumn c, ForeignKeyRelation fk)
        {
            ColumnInfo ci = TableInfo.Columns[c.DataField];
            //DbConnection con = ConnectionManager.Connection;
            TableInfo pkTable = SchemaUtility.GetTable(fk.PKTable);
            //int pkRowsCount = SqlHelper.GetRowsCount(pkTable.FullName);
            string decColumn = pkTable.GetNameColumn().ColumnName;

            string select = "select " + fk.PKField + "," + decColumn + " as [_col_] from " + pkTable.FullName;
            //if (ci.ColumnMode == ColumnViewMode.Check)
            //{
            //    UniCheckBoxList jcb = new UniCheckBoxList();
            //    if (ci.readOnly)
            //        jcb.Enabled = false;
            //    jcb.FieldName = c.DataField;


            //    var dr = SqlHelper.ExecuteReader(select);
            //    jcb.Items.Add(new ListItem(null, null));
            //    while (dr.Read())
            //        jcb.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));


            //    return jcb;
            //}
            //else if (ci.ColumnMode == ColumnViewMode.Radio)
            //{
            //    JcoRadioButtonList jrl = new JcoRadioButtonList();
            //    if (ci.readOnly)
            //        jrl.Enabled = false;

            //    if (ci.RadioColumnCount != -1)
            //        jrl.RepeatColumns = ci.RadioColumnCount;
            //    jrl.FieldName = c.DataField;

            //    var dr = SqlHelper.ExecuteReader(select);
            //    jrl.Items.Add(new ListItem(null, null));
            //    while (dr.Read())
            //        jrl.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));

            //    return jrl;

            //}
            //else if (ci.ColumnMode == ColumnViewMode.LookUp)
            //{
            UniSearchBox jrl = new UniSearchBox();
            if (ci.readOnly)
                jrl.Enabled = false;
            jrl.FieldName = c.DataField;
            jrl.SelectCommand = select;
            var dr = SqlHelper.ExecuteReader(select);
            jrl.Items.Add(new Telerik.Web.UI.RadComboBoxItem(null, null));
            while (dr.Read())
                jrl.Items.Add(new Telerik.Web.UI.RadComboBoxItem(dr[1].ToString(), dr[0].ToString()));
            dr.Close();
            return jrl;

            //}

            UniDropDownList drp = new UniDropDownList();
            if (ci.readOnly)
                drp.Enabled = false;
            drp.FieldName = c.DataField;

            var dr1 = SqlHelper.ExecuteReader(select);
            drp.Items.Add(new ListItem(null, null));
            while (dr1.Read())
                drp.Items.Add(new ListItem(dr1[1].ToString(), dr1[0].ToString()));
            dr1.Close();

            return drp;

        }

        private Control CreataNormalDataControl(DataColumn c)
        {
            ColumnInfo ci = TableInfo.Columns[c.DataField];

            //if (ci.ColumnMode != ColumnViewMode.None)
            //{
            //    if (ci.ColumnMode == ColumnViewMode.Date)
            //    {
            //        UniDatePicker jdp = new UniDatePicker();

            //        if (ci.readOnly)
            //            jdp.ReadOnly = true;
            //        jdp.Width = 95;
            //        jdp.FieldName = c.DataField;
            //        return jdp;
            //    }
            //    else if (DbTypeUtility.IsNumericType(ci.DataType) && ci.ColumnMode == ColumnViewMode.Lable)
            //    {
            //        UniLabel jl = new UniLabel();
            //        jl.FieldName = c.DataField;
            //        return jl;
            //    }
            //    else if (ci.ColumnMode == ColumnViewMode.Edit)
            //    {
            //UniTextBox jl = new UniTextBox();
            //if (ci.readOnly)
            //    jl.ReadOnly = true;
            //jl.FieldName = c.DataField;
            //return jl;
            //}
            //else if (ci.ColumnMode == ColumnViewMode.Memo)
            //{
            //    UniTextBox jl = new UniTextBox();
            //    if (ci.readOnly)
            //        jl.ReadOnly = true;
            //    jl.TextMode = TextBoxMode.MultiLine;
            //    jl.Width = 240;
            //    jl.FieldName = c.DataField;
            //    return jl;
            //}
            //else if (ci.ColumnMode == ColumnViewMode.Radio)
            //{
            //    JcoRadioButtonList jrl = new JcoRadioButtonList();
            //    if (ci.readOnly)
            //        jrl.Enabled = false;
            //    if (ci.RadioColumnCount != -1)
            //        jrl.RepeatColumns = ci.RadioColumnCount;
            //    jrl.FieldName = c.DataField;
            //    string[] item = ci.ListValue.Split('،');
            //    for (int i = 0; i < item.Length; i++)
            //        jrl.Items.Add(new ListItem(item[i], Convert.ToString(i + 1)));
            //    return jrl;
            //}

            //else if (ci.ColumnMode == ColumnViewMode.Check)
            //{
            //    UniCheckBoxList jrl = new UniCheckBoxList();
            //    if (ci.readOnly)
            //        jrl.Enabled = false;

            //    if (ci.RadioColumnCount != -1)
            //        jrl.RepeatColumns = ci.RadioColumnCount;
            //    jrl.FieldName = c.DataField;
            //    string[] item = ci.ListValue.Split('،');
            //    for (int i = 0; i < item.Length; i++)
            //        jrl.Items.Add(new ListItem(item[i], Convert.ToString(i + 1)));
            //    return jrl;
            //}
            //}
            //else
            //{

            if (ci.DataType == DbType.DateTime || ci.DataType == DbType.Date || ci.DataType == DbType.DateTime2 || ci.DataType == DbType.DateTimeOffset)
            {
                UniDatePicker dp = new UniDatePicker();
                dp.FieldName = c.DataField;
                dp.Width = 100;
                if (ci.DataType == DbType.Date)
                    dp.Mode = DatePickerMode.Date;
                return dp;
            }
            else if (DbTypeUtility.IsNumericType(ci.DataType))//&& ci.ColumnMode != ColumnViewMode.Lable)
            {
                UniNumericBox n = new UniNumericBox();
                n.FieldName = c.DataField;
                n.Width = 90;
                if (ci.readOnly)
                    n.ReadOnly = true;
                if (DbTypeUtility.IsIntegerType(ci.DataType))
                {
                    n.NumberFormat.GroupSeparator = "";
                    n.NumberFormat.DecimalDigits = 0;
                }
                else if (ci.Scale == 0)
                    n.NumberFormat.DecimalDigits = 2;
                else
                    n.NumberFormat.DecimalDigits = ci.Scale;
                return n;
            }
            else if (ci.DataType == System.Data.DbType.Boolean)
            {
                UniCheckBox chk = new UniCheckBox();
                if (ci.readOnly)
                    chk.Enabled = false;
                chk.FieldName = c.DataField;
                return chk;
            }
            else if (ci.DataType == System.Data.DbType.Binary)
            {
                return new Control();
            }
            //}
            UniTextBox txt = new UniTextBox();
            txt.FieldName = c.DataField;
            if (ci.readOnly)
                txt.ReadOnly = true;
            if (ci.Length == 0)
                txt.Width = 90;
            else if (ci.Length < 10)
                txt.Width = 90;
            else if (ci.Length > 150)
            {
                txt.TextMode = TextBoxMode.MultiLine;
                txt.Width = 240;
            }
            else if (ci.Length > 40)
                txt.Width = 240;
            else
                txt.Width = 6 * ci.Length;
            return txt;

        }

        #endregion Create Columns

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void CreateChildControls()
        {

            Controls.Add(uxToolbar);

            panel = new Panel();
            panel.ID = "toolbarPanel";
            Controls.Add(panel);
            uxToolbar.PanelID = "toolbarPanel";
            if (tableName != null)
            {
                uxToolbar.TableName = tableName;
                tableName = null;
            }
            if (sortExpression != null)
            {
                uxToolbar.SortExpression = sortExpression;
                sortExpression = null;
            }
            if (filterExpression != null)
            {
                uxToolbar.FilterExpression = filterExpression;
                filterExpression = null;
            }
            if (keyFieldName != null)
            {
                uxToolbar.KeyFieldName = keyFieldName;
                keyFieldName = null;
            }

            //uxToolbar.Showlable = ShowToolbar;


            uxGrid = new UniDataGrid();
            uxGrid.TableInfo = TableInfo;
            uxGrid.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(uxGrid_ItemCommand);
            Controls.Add(uxGrid);
            ChildControlsCreated = true;

            base.CreateChildControls();
            if (Page.Trace.IsEnabled)
                Page.Trace.Write("JcoDataForm.CreateChildControls()");
        }

        void uxGrid_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "RowClick")
            {
                Mode = DataFormMode.SingleRecord;
                Initialize();
                Condition = uxToolbar.KeyFieldName + "=" + (e.Item as GridDataItem).OwnerTableView.DataKeyValues[e.Item.ItemIndex][(e.Item as GridDataItem).OwnerTableView.DataKeyNames[0]];
                uxToolbar.SearchForm();
                Condition = null;
            }
        }



        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.Trace.Write("JcoDataForm.OnPreRender()");
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Page.Trace.IsEnabled)
                Page.Trace.Write("JcoDataForm.RenderContents()");

            if (Mode == DataFormMode.List)
            {
                uxToolbar.ShowFirstButton = uxToolbar.ShowLastButton = uxToolbar.ShowNewButton
                    = uxToolbar.ShowNextButton = uxToolbar.ShowPreviousButton = uxToolbar.ShowSaveButton = false;
            }
            else
                uxToolbar.ShowFirstButton = uxToolbar.ShowLastButton = uxToolbar.ShowNewButton
                    = uxToolbar.ShowNextButton = uxToolbar.ShowPrintButton = uxToolbar.ShowSearchWindowButton
                    = uxToolbar.ShowFormSearchButton = uxToolbar.ShowPreviousButton = uxToolbar.ShowSaveButton = true;

            //if (!ShowToolbar)
            //{
            //    uxToolbar.ShowFirstButton = uxToolbar.ShowLastButton = uxToolbar.ShowDeleteButton
            //     = uxToolbar.ShowNextButton = uxToolbar.ShowPrintButton = uxToolbar.ShowSearchWindowButton
            //        = uxToolbar.ShowFormSearchButton = uxToolbar.ShowPreviousButton = uxToolbar.ShowPreviousButton = false;
            //}

            //if (ShowToolbar)
            //{
            uxToolbar.RenderControl(writer);
            //}


            // fieldOption.Sort(delegate(FieldOption p1, FieldOption p2) { return (p1.column.panelName == null ? "" : p1.column.panelName).CompareTo(p2.column.panelName == null ? "" : p2.column.panelName); }); 

            if (Mode == DataFormMode.SingleRecord)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                int i = 0;
                foreach (var c in dataControls)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.WriteEncodedText(TableInfo.Columns[columns[i].DataField].Title);
                    writer.WriteEncodedText(": ");
                    writer.RenderEndTag();

                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    c.RenderControl(writer);
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                    i++;
                }
            }
            else
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                uxGrid.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
        }

    }
    public enum DataFormMode
    {
        List,
        SingleRecord,
    }
}
