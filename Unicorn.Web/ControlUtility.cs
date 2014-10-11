using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace Unicorn.Web
{
    public class ControlUtility
    {
        /// <summary>
        /// اولين ستون گريدويو را به عنوان ستون رديف شماره دهي مي نمايد.
        /// اين متد بايد در oninit صفحه صدا زده شود.
        /// </summary>
        /// <param name="gridView"></param>
        public static void SetRowColumn(GridView gridView)
        {
            FillGridViewRowCloumn(gridView);
            gridView.DataBound += new EventHandler(grid_view_DataBound);
        }


        private static void FillGridViewRowCloumn(GridView gridView)
        {
            for (int i = 0; i < gridView.Rows.Count; i++)
            {
                int rowIndex = gridView.PageIndex * gridView.PageSize + i + 1;
                gridView.Rows[i].Cells[0].Text = rowIndex.ToString();
            }
        }

        //private static void FillGridViewRowCloumn(RadGrid gridView)
        //{
        //    for (int i = 0; i < gridView.MasterTableView.Items.Count; i++)
        //    {
        //        int rowIndex = gridView.MasterTableView.CurrentPageIndex * gridView.MasterTableView.PageSize + i + 1;
        //        gridView.Items[i].Cells[2].Text = rowIndex.ToString();
        //    }
        //}

        public static void grid_view_DataBound(object sender, EventArgs e)
        {
            GridView gridView = (GridView)sender;
            //int i1 = gridView.PageIndex * gridView.PageSize;
            FillGridViewRowCloumn(gridView);
        }

        public static Control FindControl(Control start, Type type)
        {
            System.Web.UI.Control foundControl;
            if (start != null)
            {
                //foundControl = start.FindControl(id);
                //if (foundControl != null)
                //return foundControl;
                foreach (Control c in start.Controls)
                {
                    if (c.GetType() == type)
                        return c;
                    foundControl = FindControl(c, type);
                    if (foundControl != null)
                        return foundControl;
                }
            }
            return null;
        }
        public static Control FindControl(Control start, string id)
        {
            System.Web.UI.Control foundControl;
            if (start != null)
            {
                foundControl = start.FindControl(id);
                if (foundControl != null)
                    return foundControl;
                foreach (Control c in start.Controls)
                {
                    foundControl = FindControl(c, id);
                    if (foundControl != null)
                        return foundControl;
                }
            }
            return null;
        }

        public delegate void ControlProcessDelegate(Control c, object o);
        public static void ProccessControls(Control start, ControlProcessDelegate fn, object o)
        {
            fn(start, o);
            foreach (Control c in start.Controls)
            {
                ProccessControls(c, fn, o);
            }
        }

        

        public static void GetChildControls(Control containerControl, List<Control> childControls)
        {
            if (containerControl.Controls.Count == 0)
                childControls.Add(containerControl);
            else
                foreach (Control control in containerControl.Controls)
                    GetChildControls(control, childControls);
        }

        public static void RegisterDatePicker(Page p)
        {
            p.ClientScript.RegisterClientScriptResource(p.GetType(), "Unicorn.Web.Resources.js.PersianDatePicker.js");
        }

        #region Print GridView

        /// <summary>
        /// onclick يك دكمه را براي پرينت گرفتن از محتواي يك گريدويو تنظيم مي كند.
        /// </summary>
        /// <param name="gridview1"></param>
        /// <param name="print_button"></param>
        /// <param name="report_title"></param>
        public static void PrintGridView(GridView gridview1, WebControl print_button, string report_title)
        {
            Page page = gridview1.Page;
            //چون بعضي جاها در اسكريپت مجبور به استفاده ار " بودم گاهي عبارت را با + به هم چسباندم.
            string script = @"
      function printbtn_onclick() {
            try {
                if (navigator.appName != 'Microsoft Internet Explorer')
                    alert('جهت چاپ بايد از جستجوگر Internet Explorer يا بالاتر استفاده نماييد.');
                var popup = window.open('', '_blank'); 
" +
"                var style = 'style=\"direction:rtl;text-align:right;font-family:Tahoma;font-size:11pt;\"'; " +
"                var gridview = '<P style=\"direction:rtl\" >' + document.getElementById('" + gridview1.ClientID + @"').outerHTML + '</P>';
                var popupBody = popup.document.body
                popupBody.style.border = 'solid 2px black'
                popupBody.style.padding = '5px';
                popupBody.innerHTML = '<P ' + style + '  >" + report_title + @"</P>' + gridview;
                popup.print();
                return false;
            }
            catch (errorinfo) {
                alert('خطا:' + errorinfo.message);
                return false;
            }
        }	
";
            string script_key = "ReportGeneratorFromGridViewScript" + print_button.GetHashCode();
            if (!page.ClientScript.IsClientScriptBlockRegistered(script_key))
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), script_key, script, true);
            print_button.Attributes.Add("onclick", "return printbtn_onclick()");
        }

        #endregion Print GridView

        #region Export GridView To Excel

        /// <summary>
        /// محتواي يك گريدويو به شكل دانلود مي فرستد. 
        /// </summary>
        /// <param name="GridView1"></param>
        //public static void ExportGridViewToExcel(GridView GridView1)
        //{
        //    HttpResponse Response = GridView1.Page.Response;
        //    ExportGridViewToExcel(GridView1, Response.OutputStream);
        //    Response.ContentType = "application/xls";
        //    Response.AppendHeader("Content-Disposition", "attachment; filename=file.xls");
        //    Response.End();
        //}

        //private static void ExportGridViewToExcel(GridView GridView1, Stream stream)
        //{
        //    Aspose.Cells.Workbook book = new Aspose.Cells.Workbook();
        //    book.Worksheets[0].DisplayRightToLeft = true;

        //    AddGridViewHeaderToExcelWorkSheet(GridView1, book);
        //    AddGridViewBodyToExcelWorkSheet(GridView1, book);
        //    //if (HasRadifColumn)
        //    //    AddGridViewRadifToExcelWorkSheet(GridView1, book);

        //    book.Save(stream, Aspose.Cells.FileFormatType.Excel2003);
        //}

        //private static void AddGridViewBodyToExcelWorkSheet(GridView GridView1, Aspose.Cells.Workbook workbook)
        //{
        //    foreach (GridViewRow gridViewRow in GridView1.Rows)
        //    {
        //        int columnIndex = 0;
        //        foreach (TableCell cell in gridViewRow.Cells)
        //        {
        //            string text = GetTextOfGridViewCell(cell);
        //            workbook.Worksheets[0].Cells[gridViewRow.RowIndex + 1, columnIndex].PutValue(text);
        //            columnIndex++;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string GetTextOfGridViewCell(TableCell cell)
        {
            string text = "";
            try
            {
                text += cell.Text;
                foreach (Control control in cell.Controls)
                {
                    if (control is Label)
                        text += ((Label)control).Text;
                    else if (control is TextBox)
                        text += ((TextBox)control).Text;
                    else if (control is LinkButton)
                        text += ((LinkButton)control).Text;
                    else if (control is Button)
                        text += ((Button)control).Text;
                    else if (control is Literal)
                        text += ((Literal)control).Text;
                    else if (control is DropDownList)
                        text += ((DropDownList)control).SelectedValue;
                    else if (control is ListBox)
                        text += ((ListBox)control).SelectedValue;
                }
                text = HttpUtility.HtmlDecode(text);
                //}
            }
            catch
            {
            }
            return text;
        }

        //private static void AddGridViewHeaderToExcelWorkSheet(GridView GridView1, Aspose.Cells.Workbook workbook)
        //{
        //    int columnIndex = 0;
        //    foreach (TableCell cell in GridView1.HeaderRow.Cells)
        //    {
        //        string text = GetTextOfGridViewCell(cell);
        //        workbook.Worksheets[0].Cells[0, columnIndex].PutValue(text);
        //        columnIndex++;
        //    }
        //}

        //private static void AddGridViewRadifToExcelWorkSheet(GridView gridView1, Aspose.Cells.Workbook workBook)
        //{
        //    foreach (GridViewRow gridViewRow in gridView1.Rows)
        //        workBook.Worksheets[0].Cells[gridViewRow.RowIndex + 1, 0].PutValue(gridViewRow.Cells[0].Text);
        //}

        #endregion Export GridView To Excel

    }
}
