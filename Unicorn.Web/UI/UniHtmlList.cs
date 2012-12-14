using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Unicorn.Web.UI
{
    public class UniHtmlList : UniHtmlListItem
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Li);
            base.RenderContents(writer);
            writer.RenderEndTag();
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Ul;
            }
        }
    }

    public class UniHtmlListItem : WebControl
    {
        public UniHtmlListItem(string text, string navigateUrl)
            : this(text)
        {
            NavigateUrl = navigateUrl;
        }
        public UniHtmlListItem(string text)
            : this()
        {
            Text = text;
        }
        public UniHtmlListItem()
        {
            Items = new List<UniHtmlListItem>();
        }

        public List<UniHtmlListItem> Items { get; set; }

        public string Text { get; set; }
        public string NavigateUrl { get; set; }
        public string Target { get; set; }
        public string Value { get; set; }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Li;
            }
        }
        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(NavigateUrl) || !string.IsNullOrEmpty(Text))
            {
                if (!string.IsNullOrEmpty(NavigateUrl) )
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(NavigateUrl));
                if (!string.IsNullOrEmpty(Target))
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, Target);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                RenderText(writer);
                writer.RenderEndTag();
            }
            //else
            //    RenderText(writer);
            if (Items.Count > 0)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (var item in Items)
                    item.Render(writer);
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
        }
        void RenderText(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(Text))
                return;
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.WriteEncodedText(Text);
            writer.RenderEndTag();
        }
                
    }
}
