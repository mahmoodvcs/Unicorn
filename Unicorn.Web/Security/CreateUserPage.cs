using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Unicorn.Web.Security.Configuration;
using System.Configuration;
using Unicorn.Web.UI;

namespace Unicorn.Web.Security.Pages
{
	public class CreateUserPage : PageWithMaster
	{
		Unicorn.Web.UI.CreateUserWizard createUserWizard;

		protected override void OnPreLoad( EventArgs e )
		{
			base.OnPreLoad( e );
  			HtmlTable table = new HtmlTable();
			table.Align = "center";
			table.CellPadding = 0;
			table.CellSpacing = 0;
			HtmlTableRow row = new HtmlTableRow();
			table.Rows.Add( row );
			HtmlTableCell cell = new HtmlTableCell();
			row.Cells.Add( cell );

            createUserWizard = new Web.UI.CreateUserWizard();
			cell.Controls.Add( createUserWizard );
			Control cph = GetContentPlaceHolder();
			if (cph != null)
                cph.Controls.Add(table);
            else
                Form.Controls.Add(table);
		}
	}
}
