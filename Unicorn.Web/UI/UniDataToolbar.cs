using System;
using System.Collections.Generic;

using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using Telerik.Web.UI;
using System.ComponentModel;
using System.Data.SqlClient;
using Unicorn.Data;
using System.Drawing;
using System.Data.Common;
using System.Data;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:UniDataToolbar runat=server></{0}:UniDataToolbar>")]
    public class UniDataToolbar : CompositeControl
    {
        public UniDataToolbar()
        {
            //Width = 200;
            //Height = 60;
            Attributes["dir"] = "rtl";
            CustomButtons = new List<ImageButton>();
        }


        //RadToolBar toolbar;
        ImageButton uxFirst, uxLast, uxPrev, uxNext, uxSearch, uxFindForm, uxSave, uxNew, uxDelete, uxPrint;

        Label uxMessage;
        Record dataRecord;

        #region Properties

        #region ButtonText



        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowLastButton
        {
            get { return (bool)(ViewState["shLast"] ?? true); }
            set { ViewState["shLast"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowDeleteButton
        {
            get { return (bool)(ViewState["shDelete"] ?? true); }
            set { ViewState["shDelete"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowSaveButton
        {
            get { return (bool)(ViewState["shSave"] ?? true); }
            set { ViewState["shSave"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowPreviousButton
        {
            get { return (bool)(ViewState["shPrev"] ?? true); }
            set { ViewState["shPrev"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowNextButton
        {
            get { return (bool)(ViewState["shNext"] ?? true); }
            set { ViewState["shNext"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowFirstButton
        {
            get { return (bool)(ViewState["shFirst"] ?? true); }
            set { ViewState["shFirst"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowPrintButton
        {
            get { return (bool)(ViewState["shPrint"] ?? true); }
            set { ViewState["shPrint"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowNewButton
        {
            get { return (bool)(ViewState["shNew"] ?? true); }
            set { ViewState["shNew"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowFormSearchButton
        {
            get { return (bool)(ViewState["shFind"] ?? true); }
            set { ViewState["shFind"] = value; }
        }
        [Bindable(true)]
        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue(true)]
        public bool ShowSearchWindowButton
        {
            get { return (bool)(ViewState["shSear"] ?? true); }
            set { ViewState["shSear"] = value; }
        }


        #endregion

        [Category("Behavior")]
        [Localizable(false)]
        [IDReferenceProperty(typeof(Panel))]
        public string PanelID
        {
            get { return (string)(ViewState["PanelID"] ?? ""); }
            set { ViewState["PanelID"] = value; }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string TableName
        {
            get { return (string)(ViewState["TableName"] ?? ""); }
            set
            {
                if (ViewState["TableName"] != null && value == ViewState["TableName"].ToString())
                    return;
                ViewState["TableName"] = value;
                TableInfoInternal = null;
            }
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
        [DefaultValue("")]
        public string SortExpression
        {
            get { return (string)(ViewState["Sort"] ?? KeyFieldName); }
            set { ViewState["Sort"] = value; }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string FilterExpression
        {
            get { return (string)(ViewState["Filter"] ?? ""); }
            set { ViewState["Filter"] = value; }
        }

        [Category("Behavior")]
        [Localizable(false)]
        [DefaultValue("")]
        public string Condition
        {
            get { return (string)(ViewState["Condition"] ?? ""); }
            set { ViewState["Condition"] = value; }
        }

        public int RowsCount
        {
            get { return (int)(ViewState["RowCount"] ?? 0); }
            private set { ViewState["RowCount"] = value; }
        }
        public int CurrentRowIndex
        {
            get { return (int)(ViewState["CurrentRowIndex"] ?? 0); }
            set { ViewState["CurrentRowIndex"] = value; }
        }
        public object CurrentKeyValue
        {
            get { return (object)(ViewState["CurrentKeyValue"] ?? null); }
            set { ViewState["CurrentKeyValue"] = value; }
        }
        public bool Showlable
        {
            get { return (bool)(ViewState["Showlable"] ?? true); }
            set { ViewState["Showlable"] = value; }
        }


        public FormState FormState
        {
            get { return (FormState)(ViewState["FormState"] ?? FormState.Empty); }
            set { ViewState["FormState"] = value; }
        }
        public bool IsNavigating { get; private set; }
        //public bool EnableAutoIncrementIdentity
        //{
        //    get { return (FormState)(ViewState["FormState"] ?? FormState.Empty); }
        //    set { ViewState["FormState"] = value; }
        //}

        [Category("Behavior")]
        [Localizable(false)]
        public List<ImageButton> CustomButtons
        {
            get;
            set;
        }

        public Record DataRecord
        {
            get { return dataRecord; }
        }
        #endregion

        #region Private properties
        public TableInfo TableInfo
        {
            get
            {
                if (TableInfoInternal == null)
                    Initialize();
                return TableInfoInternal;
            }
        }
        private TableInfo TableInfoInternal
        {
            get
            {
                return (TableInfo)(ViewState["TableInfo"] ?? null);
            }
            set { ViewState["TableInfo"] = value; }
        }


        private string UserCondition
        {
            get { return (string)(ViewState["UserCondition"] ?? null); }
            set { ViewState["UserCondition"] = value; }
        }


        #endregion Private properties

        #region Events
        public event CancelEventHandler RowUpdating;
        public event CancelEventHandler RowInserting;
        public event CancelEventHandler RowDeleting;

        public event EventHandler CurrentRowChanged;
        public event EventHandler RowUpdated;
        public event EventHandler RowInserted;
        public event EventHandler RowDeleted;

        public event EventHandler SearchClicked;
        public event EventHandler PrintClicked;


        public void OnPrintClicked()
        {
            OnPrintClicked(this, EventArgs.Empty);
        }
        public virtual void OnPrintClicked(object sender, EventArgs e)
        {
            if (PrintClicked != null)
                PrintClicked(sender, e);
        }
        public void OnCurrentRowChanged()
        {
            OnCurrentRowChanged(this, EventArgs.Empty);
        }
        public virtual void OnCurrentRowChanged(object sender, EventArgs e)
        {
            if (CurrentRowChanged != null)
                CurrentRowChanged(sender, e);
        }
        public void OnSearchClicked()
        {
            OnSearchClicked(this, EventArgs.Empty);
        }
        public virtual void OnSearchClicked(object sender, EventArgs e)
        {
            if (SearchClicked != null)
                SearchClicked(sender, e);
        }

        public void OnRowDeleted()
        {
            OnRowDeleted(this, EventArgs.Empty);
        }
        public virtual void OnRowDeleted(object sender, EventArgs e)
        {
            if (RowDeleted != null)
                RowDeleted(sender, e);
        }
        public void OnRowUpdating()
        {
            OnRowUpdating(this, new CancelEventArgs());
        }
        public virtual void OnRowUpdating(object sender, CancelEventArgs e)
        {
            if (RowUpdating != null)
                RowUpdating(sender, e);
        }

        public void OnRowDeleting()
        {
            OnRowDeleting(this, new CancelEventArgs());
        }
        public virtual void OnRowDeleting(object sender, CancelEventArgs e)
        {
            if (RowDeleting != null)
                RowDeleting(sender, e);
        }

        public void OnRowInserting()
        {
            OnRowInserting(this, new CancelEventArgs());
        }
        public virtual void OnRowInserting(object sender, CancelEventArgs e)
        {
            if (RowInserting != null)
                RowInserting(sender, e);
        }


        public void OnRowUpdated()
        {
            OnRowUpdated(this, EventArgs.Empty);
        }
        public virtual void OnRowUpdated(object sender, EventArgs e)
        {
            if (RowUpdated != null)
                RowUpdated(sender, e);
        }
        public void OnRowInserted()
        {
            OnRowInserted(this, EventArgs.Empty);
        }
        public virtual void OnRowInserted(object sender, EventArgs e)
        {
            if (RowInserted != null)
                RowInserted(sender, e);
        }

        #endregion


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (TableInfo == null)
                return;
            RowsCount = (int)SqlHelper.ExecuteScaler("select count(*) from " + TableInfo.FullName);

        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(TableName))
                return;
                //throw new Exception("TableName is not specified.");
            Page.Trace.Write("JcoDataToolbar.Initialize()");
            if (TableInfoInternal == null || TableInfoInternal.TableName != TableName)
                TableInfoInternal =
                    Data.SchemaUtility.GetTable(TableName);
            RowsCount = (int)SqlHelper.ExecuteScaler("select count(*) from " + TableInfo.FullName);
        }
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void CreateChildControls()
        {
            Page.Trace.Write("JcoDataToolbar.CreateChildControls()");
            //toolbar = new RadToolBar();
            //toolbar.EnableEmbeddedSkins = true;
            //toolbar.Skin = "Vista";
            //toolbar.Width = ;
            //toolbar.Height = Height;
            //Controls.Add(toolbar);
            //toolbar.ButtonClick += new RadToolBarEventHandler(toolbar_ButtonClick);

            //uxFirst = new RadToolBarButton(FirstButtonText);
            uxFirst = new ImageButton();
            //uxFirst.Value = "First";
            uxFirst.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.First.png");
            //uxFirst.ImagePosition = ToolBarImagePosition
            Controls.Add(uxFirst);
            //toolbar.Items.Add(uxFirst);

            //uxPrev = new RadToolBarButton(PreviousButtonText);
            uxPrev = new ImageButton();
            //uxPrev.Value = "Prev";
            uxPrev.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Prev.png");
            Controls.Add(uxPrev);
            //toolbar.Items.Add(uxPrev);

            //uxNext = new RadToolBarButton(NextButtonText);
            uxNext = new ImageButton();
            //uxNext.Value = "Next";
            uxNext.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Next.png");
            Controls.Add(uxNext);

            //uxLast = new RadToolBarButton(LastButtonText);
            uxLast = new ImageButton();
            //uxLast.Value = "Last";
            uxLast.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Last.png");
            Controls.Add(uxLast);

            //uxSave = new RadToolBarButton(SaveButtonText);
            uxSave = new ImageButton();
            //uxSave.Value = "Save";
            uxSave.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Save.png");
            Controls.Add(uxSave);

            //uxNew = new RadToolBarButton(NewButtonText);
            uxNew = new ImageButton();
            //uxNew.Value = "New";
            uxNew.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.New.png");
            Controls.Add(uxNew);

            //uxFirst = new RadToolBarButton(FirstButtonText);
            //uxFirst.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Jco.WebControls.Resources.Images.First.png");
            //toolbar.Items.Add(uxFirst);

            uxDelete = new ImageButton();
            uxDelete.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Delete.png");
            uxDelete.OnClientClick = "if(!confirm('آيا مطمئن هستيد؟')) return false;";
            Controls.Add(uxDelete);

            uxSearch = new ImageButton();
            uxSearch.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.SearchWin.png");
            //uxSearch.OnClientClick = "if(!confirm('آيا مطمئن هستيد؟')) return false;";
            Controls.Add(uxSearch);

            uxFindForm = new ImageButton();
            uxFindForm.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.FormFind.png");
            Controls.Add(uxFindForm);


            uxPrint = new ImageButton();
            uxPrint.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "Unicorn.Web.Resources.Images.Print.png");
            Controls.Add(uxPrint);


            uxFirst.Click += new ImageClickEventHandler(uxFirst_Click);
            uxPrev.Click += new ImageClickEventHandler(uxPrev_Click);
            uxNext.Click += new ImageClickEventHandler(uxNext_Click);
            uxLast.Click += new ImageClickEventHandler(uxLast_Click);
            uxSave.Click += new ImageClickEventHandler(uxSave_Click);
            uxNew.Click += new ImageClickEventHandler(uxNew_Click);
            uxDelete.Click += new ImageClickEventHandler(uxDelete_Click);
            uxSearch.Click += new ImageClickEventHandler(uxSearch_Click);
            uxFindForm.Click += new ImageClickEventHandler(uxFindForm_Click);
            uxPrint.Click += new ImageClickEventHandler(uxPrint_Click);

            uxMessage = new Label();
            uxMessage.Style[HtmlTextWriterStyle.Display] = "none";
            uxMessage.BackColor = Color.LightYellow;
            uxMessage.ForeColor = Color.Red;
            uxMessage.EnableViewState = false;
            Controls.Add(uxMessage);

            foreach (ImageButton img in CustomButtons)
            {
                Controls.Add(img);
            }

            base.CreateChildControls();
            ChildControlsCreated = true;
        }

        void uxFindForm_Click(object sender, ImageClickEventArgs e)
        {
            if (!SearchForm())
                uxMessage.Text = "داده اي پيدا نشد.";
        }

        void uxSearch_Click(object sender, ImageClickEventArgs e)
        {
            ClearForm();
            OnSearchClicked();
        }

        void uxPrint_Click(object sender, ImageClickEventArgs e)
        {
            OnPrintClicked();
        }

        void uxDelete_Click(object sender, ImageClickEventArgs e)
        {
            if (CurrentRowIndex < 0)
            {
                uxMessage.Text = "هیچ سطری انتخاب نشده است.";
                return;
            }
            CancelEventArgs e1 = new CancelEventArgs();
            OnRowDeleting(this, e1);
            if (e1.Cancel == true)
                return;
            Record r = new Record(TableName);
            r.Keys[KeyFieldName] = CurrentKeyValue;
            r.Delete();
            RowsCount--;
            if (CurrentRowIndex == RowsCount - 1)
                CurrentRowIndex--;
            FillControls();
            uxMessage.Text = "حذف با موفقيت انجام شد.";
            //  OnCurrentRowChanged();
            OnRowDeleted();
            ClearForm();
            FormState = FormState.New;
        }

        void uxNext_Click(object sender, ImageClickEventArgs e)
        {
            IsNavigating = true;
            if (CurrentRowIndex < RowsCount - 1)
                CurrentRowIndex++;
            FillControls();
            FormState = FormState.Edit;
            OnCurrentRowChanged();
        }

        void uxSave_Click(object sender, ImageClickEventArgs e)
        {
            CancelEventArgs e1 = new CancelEventArgs();
            OnRowUpdating(this, e1);
            if (e1.Cancel == true)
                return;
            SaveForm();
            if (FormState == FormState.Edit)
                OnRowUpdated();
            else
                OnRowInserted();

            FormState = FormState.Edit;
        }

        void uxLast_Click(object sender, ImageClickEventArgs e)
        {
            IsNavigating = true;
            CurrentRowIndex = RowsCount - 1;
            FillControls();
            FormState = FormState.Edit;
            OnCurrentRowChanged();
        }

        void uxNew_Click(object sender, ImageClickEventArgs e)
        {
            uxMessage.Text = "آماده برای درج ";
            ClearForm();
            FormState = FormState.New;
            OnCurrentRowChanged();
        }

        void uxPrev_Click(object sender, ImageClickEventArgs e)
        {
            IsNavigating = true;
            if (CurrentRowIndex > 0)
                CurrentRowIndex--;
            FillControls();
            FormState = FormState.Edit;
            OnCurrentRowChanged();
        }

        void uxFirst_Click(object sender, ImageClickEventArgs e)
        {
            IsNavigating = true;
            CurrentRowIndex = 0;
            FillControls();
            FormState = FormState.Edit;
            OnCurrentRowChanged();
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            //if (Width.IsEmpty)
            //    Width = toolbar.Width;
            //else
            //    toolbar.Width = Width;
            //if (Height.IsEmpty)
            //    Height = toolbar.Width;
            //else
            //    toolbar.Height = Height;
            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (ShowFirstButton)
                uxFirst.RenderControl(writer);
            if (ShowPreviousButton)
                uxPrev.RenderControl(writer);
            if (ShowNextButton)
                uxNext.RenderControl(writer);
            if (ShowLastButton)
                uxLast.RenderControl(writer);
            if (ShowSaveButton)
                uxSave.RenderControl(writer);
            if (ShowNewButton)
                uxNew.RenderControl(writer);
            if (ShowDeleteButton)
                uxDelete.RenderControl(writer);
            if (ShowFormSearchButton)
                uxFindForm.RenderControl(writer);
            if (ShowPrintButton)
                uxPrint.RenderControl(writer);
            if (ShowSearchWindowButton)
                uxSearch.RenderControl(writer);
            foreach (ImageButton img in CustomButtons)
            {
                img.RenderControl(writer);
            }



            writer.Write(" ");
            // uxDelete.RenderControl(writer);
            if (Showlable)
                writer.WriteEncodedText(" سطر " + (CurrentRowIndex + 1).ToString() + " از " + RowsCount.ToString());
            uxMessage.RenderControl(writer);


        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(GetType(), "Unicorn.Web.Resources.js.DataToolbar.js");
            Page.ClientScript.RegisterStartupScript(GetType(), uxMessage.ClientID, "JcoDataToolbar_CheckMessageLabel('" + uxMessage.ClientID + "');", true);
            base.OnPreRender(e);
        }



        #region Data Manipulation
        private Control GetStartControl()
        {
            Control start = null;
            if (!string.IsNullOrEmpty(PanelID))
                start = ControlUtility.FindControl(Page, PanelID);
            return start ?? Page;
        }
        private void SaveForm()
        {
            dataRecord = new Record(TableName);
            Control start = GetStartControl();
            ControlUtility.ProccessControls(start, new ControlUtility.ControlProcessDelegate(SaveControl), null);
            if (FormState == FormState.Edit)
            {
                dataRecord.Keys[KeyFieldName] = CurrentKeyValue;
                dataRecord.Update();
                uxMessage.Text = "ويرايش با موفقيت انجام شد.";
            }
            else
            {
                dataRecord.Insert();
                RowsCount++;
                uxMessage.Text = "ذخيره با موفقيت انجام شد.";
            }
  
        }
        private void SaveControl(Control c, object o)
        {
            IDataControl d = c as IDataControl;
            if (d != null && TableInfo.Columns[d.FieldName] != null && !TableInfo.Columns[d.FieldName].IsIdentity)
                dataRecord[d.FieldName] = d.Value;
            //Unicorn.Web.WebUtility.ShowMessageBox("اطلاعات با موفقيت ذخيره شد", Page);
        }

        void ClearForm()
        {
            UserCondition = null;
            CurrentKeyValue = null;
            CurrentRowIndex = -1;
            Control start = GetStartControl();
            ControlUtility.ProccessControls(start, new ControlUtility.ControlProcessDelegate(ClearControl), null);
        }

        private void ClearControl(Control c, object o)
        {
            IDataControl d = c as IDataControl;
            if (d != null && TableInfo.Columns[d.FieldName] != null)
                d.Value = null;
        }

        private void FillControls()
        {
            if (CurrentRowIndex == -1)
            {
                ClearForm();
                return;
            }

            var dr = SqlHelper.ExecuteReader(DBDataUtility.GetPagedSelectStatement(TableInfo, SortExpression,
                CurrentRowIndex, 1, UserCondition));
            if (!dr.Read())
            {
                dr.Close();
                CurrentKeyValue = null;
                return;
            }
            FillFromReader(dr);
            dr.Close();
        }
        private void FillFromReader(IDataReader dr)
        {
            RowsCount = (int)SqlHelper.ExecuteScaler("select count(*) from " + TableInfo.FullName + " where 1=1 " + UserCondition);
            dataRecord = new Record();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                dataRecord[dr.GetName(i)] = (dr[i] == DBNull.Value ? null : dr[i]);
            }
            CurrentKeyValue = dataRecord[KeyFieldName];
            Control start = GetStartControl();
            ControlUtility.ProccessControls(Page, new ControlUtility.ControlProcessDelegate(FillControl), null);
        }
        private void FillControl(Control c, object o)
        {
            IDataControl d = c as IDataControl;
            if (d != null && TableInfo.Columns[d.FieldName] != null)
                d.Value = dataRecord[d.FieldName];
        }
        public bool SearchForm()
        {
            string sel = "select * from " + TableInfo.FullName + " where 1=1 ";
            List<string> conditions = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            ControlUtility.ProccessControls(Page,
                SetSearchCondition,
                new object[] { conditions, parameters });
            if (conditions.Count > 0)
            {
                sel += " and  " + string.Join(" and ", conditions.ToArray());
            }

            if (Condition != "")
                sel += " and " + Condition;
            var dr = SqlHelper.ExecuteReader(sel, parameters.ToArray());

            foreach (var c in conditions.ToArray())
                foreach (var p in parameters.ToArray())
                    if (c.Split('=')[1].ToLower() == p.ParameterName.ToLower())
                        UserCondition += " and " + c.Replace(c.Split('=')[1], p.SqlValue.ToString());

            if (Condition != "")
                UserCondition += " and " + Condition;

            if (!dr.Read())
            {
                dr.Close();
                return false;
            }


            FillFromReader(dr);
            dr.Close();
            return true;
        }
        private void SetSearchCondition(Control c, object o)
        {
            IDataControl d = c as IDataControl;
            if (d == null || TableInfo.Columns[d.FieldName] == null
               || d.Value == null || d.Value.ToString() == string.Empty)
                return;
            if (d.Value is bool && (bool)d.Value == false)
                return;
            object[] o2 = (object[])o;
            List<string> conditions = (List<string>)o2[0];
            List<SqlParameter> parameters = (List<SqlParameter>)o2[1];
            string n = "@" + d.FieldName;
            parameters.Add(new SqlParameter(n, d.Value));
            conditions.Add(d.FieldName + "=" + n);
        }
        #endregion Data Manipulation

    }

    public enum FormState
    {
        Empty,
        Edit,
        New,
    }
}