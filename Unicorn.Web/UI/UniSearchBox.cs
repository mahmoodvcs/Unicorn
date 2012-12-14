using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Unicorn.Data;

namespace Unicorn.Web.UI
{
    public class UniSearchBox : RadComboBox, IDataControl, ISecurityControl
    {
        #region Properties And Fields

        public string SelectCommand
        {
            get { return (string)(ViewState["SelectCommand"] ?? ""); }
            set { ViewState["SelectCommand"] = value; }
        }


        public object Value
        {
            get
            {
                object obj = this.SelectedValue;
                return obj;
            }
            set
            {
                if (!DesignMode)
                {
                    this.ClearSelection();
                    this.Text = "";
                    this.Items.Clear();
                    if (value != null)
                        LoadItemByValue(value.ToString());
                }
            }
        }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string FieldName { get; set; }

        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        public string SecurityKey { get; set; }

        #endregion Properties And Fields

        #region Methods

        public UniSearchBox()
        {
            // SelectCommand = FieldName = "";
            this.EnableLoadOnDemand = true;//->this.AllowCustomText = true;
            this.ShowMoreResultsBox = true;
            this.EnableVirtualScrolling = true;
            this.MarkFirstMatch = true;
            this.LoadingMessage = "لطفا صبر كنيد...";
            this.EnableVirtualScrolling = true;
            this.ItemsPerRequest = 10;
            this.Attributes["dir"] = "rtl";
        }

        private void LoadItemByValue(string value)
        {
            string[] fields = DBDataUtility.GetColumnsFromSelectCommand(SelectCommand);
            if (fields.Length == 0)
                throw new Exception("ErrorCode : 1390/04/14-19:14");
            string strSelect = @"
Select 
    *
From (
    " + DBDataUtility.AddTopToSelectComand(SelectCommand) + @"
    ) Temp" + Unicorn.Web.Utility.GetRandomGUID() + @"
Where 
    [" + fields[0] + "] = @id ";
            SqlParameter prm = new SqlParameter("id", value);
            DataTable table = SqlHelper.ExecuteCommand(strSelect, prm);
            if (table.Rows.Count == 0)
                throw new Exception("مفدار «" + value.ToString() + "» در كنترل «" + this.ID + "» يافت نشد!");
            SetItems(table, 0, 1);
        }

        private void SetItems(DataTable dataTable, int itemOffset, int endOffset)
        {
            this.Items.Clear();
            for (int i = itemOffset; i < endOffset; i++)
                this.Items.Add(new RadComboBoxItem(dataTable.Rows[i][1].ToString(), dataTable.Rows[i][0].ToString()));
        }

        private string GetSelectCommand(string text, string selectCommand, out SqlParameter[] sqlParameteres)
        {
            List<SqlParameter> sqlParamesList = new List<SqlParameter>();
            if (text.Trim() != "")
            {
                string[] columns = DBDataUtility.GetColumnsFromSelectCommand(selectCommand);
                selectCommand = DBDataUtility.AddTopToSelectComand(selectCommand);
                selectCommand = @"
Select * 
From (
    " + selectCommand + @" 
) Temp" + Guid.NewGuid().ToString().Replace("-", "_");
                List<string> conditions = new List<string>();
                foreach (string column in columns)
                {
                    conditions.Add("dbo.ReplaceYK(" + column + ") like N'%' + dbo.ReplaceYK(@" + column + ") + N'%'");
                    sqlParamesList.Add(new SqlParameter(column, text.Trim()));
                }
                if (conditions.Count == 0)
                    throw new Exception();
                selectCommand += @"
Where
    " + string.Join(@"
Or ", conditions.ToArray());
            }
            sqlParameteres = sqlParamesList.ToArray();

            return selectCommand;
        }

        private static string GetStatusMessage(int offset, int total)
        {
            if (total <= 0)
                return "موردي يافت نشد";
            return string.Format("مورد <b>1</b>تا<b>{0}</b> از <b>{1}</b>", offset, total);
        }

        #endregion Methods

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (string.IsNullOrEmpty(DataSourceID))
                this.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(JcoSearchBox_ItemsRequested);
        }

        void JcoSearchBox_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            SqlParameter[] sqlParams;
            string strSelect = GetSelectCommand(e.Text, this.SelectCommand, out sqlParams);
            DataTable data = SqlHelper.ExecuteCommand(strSelect, sqlParams);
            int itemOffset = e.NumberOfItems;
            int endOffset = Math.Min(itemOffset + ItemsPerRequest, data.Rows.Count);
            e.EndOfItems = endOffset == data.Rows.Count;
            SetItems(data, itemOffset, endOffset);
            e.Message = GetStatusMessage(endOffset, data.Rows.Count);
        }

        /* protected override void OnItemsRequested(RadComboBoxItemsRequestedEventArgs eargs)
         {
             //            base.OnItemsRequested(eargs);
             SqlParameter[] sqlParams;
             string strSelect = GetSelectCommand(eargs.Text, this.SelectCommand, out sqlParams);
             DataTable data = SqlHelper.ExecuteCommand(strSelect, sqlParams);
             int itemOffset = eargs.NumberOfItems;
             int endOffset = Math.Min(itemOffset + ItemsPerRequest, data.Rows.Count);
             eargs.EndOfItems = endOffset == data.Rows.Count;
             SetItems(data, itemOffset, endOffset);
             eargs.Message = GetStatusMessage(endOffset, data.Rows.Count);
         }*/

        protected override void RenderContents(HtmlTextWriter writer)
        {
            //base.ge
            base.RenderContents(writer);
        }

        #endregion Event Handlers
    }
}
