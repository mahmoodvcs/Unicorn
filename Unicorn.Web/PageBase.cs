using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Unicorn.Web
{
	public class PageBase : Page
	{
        public PageBase()
            : this(false)
        {
        }
        public PageBase(bool hasMasterPage)
        {
            this.hasMasterPage = hasMasterPage;
            //if (!hasMasterPage)
            {
                HtmlGenericControl html = new HtmlGenericControl("html");
                Controls.Add(html);
                head = new HtmlHead();
                html.Controls.Add(head);
                
                //title = new HtmlTitle();
                //title.Text = "my page";
                //head.Controls.Add( title );
                body = new HtmlGenericControl("body");
                body.Attributes["dir"] = "rtl";
                html.Controls.Add(body);

                form = new HtmlForm();
                form.Attributes.Add("runat", "server");
                form.ID = form.Name = "Form1";
                body.Controls.Add(form);
            }
        }
        protected bool hasMasterPage;
		private HtmlForm form;
		private HtmlGenericControl body;
		private HtmlHead head;
		private HtmlTitle title;

		public HtmlGenericControl Body
		{
			get { return body; }
		}
        public new HtmlForm Form
        {
            get
            {
				if (hasMasterPage && Master != null)
                    return base.Form;
                else
                    return form;
            }
        }
        public new HtmlHead Header
        {
            get
            {
				if (hasMasterPage && Master != null)
                    return base.Header;
                else
                    return head;
            }
        }
    }
}
