using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web;
using Telerik.Web.UI;

namespace Unicorn.Web.UI
{
	class DateTemplate : ITemplate
	{
		public DateTemplate(string dataField)
		{
			this.dataField = dataField;
		}
		string dataField;

		public void InstantiateIn(Control container)
		{
			Label l = new Label();
			l.DataBinding += new EventHandler(Label_DataBinding);
			container.Controls.Add(l);
		}

		void Label_DataBinding(object sender, EventArgs e)
		{
			Label l = (Label)sender;
            GridDataItem row = (GridDataItem)l.NamingContainer;
			l.Text = PersianDateTimeConverter.MiladiToShamsi((DateTime)DataBinder.Eval(row.DataItem, dataField)).ToString();
		}
	}
}
