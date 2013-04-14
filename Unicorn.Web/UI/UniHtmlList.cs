using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Unicorn.Web.UI
{
    public class UniHtmlList : WebControl
    {
        public UniHtmlList()
        {
            //Items = new List<UniHtmlListItem>();
            //controls = new UniHtmlListItemCollection(this);
        }
        //protected override void RenderContents(HtmlTextWriter writer)
        //{
        //    //writer.RenderBeginTag(HtmlTextWriterTag.Li);
        //    //base.RenderContents(writer);
        //    //writer.RenderEndTag();
        //}
        //public List<UniHtmlListItem> Items { get; set; }

        //UniHtmlListItemCollection controls;

        //public override ControlCollection Controls
        //{
        //    get
        //    {
        //        return controls;
        //    }
        //}

        public ControlCollection Items { get { return Controls; } }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Ul;
            }
        }

        public void Add(UniHtmlListItem child)
        {
            Controls.Add(child);
        }
        public override string ToString()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, System.Text.Encoding.Unicode);
            HtmlTextWriter w = new HtmlTextWriter(sw);
            Render(w);
            w.Flush();
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms, true);
            string s = sr.ReadToEnd();
            sr.Close();
            return s;
        }
        //protected override void Render(HtmlTextWriter writer)
        //{
        //    base.Controls.Clear();
        //    foreach (Control c in Controls)
        //    {
        //        base.Controls.Add(c);
        //    }
        //    base.Render(writer);
        //}

    }

    public class UniHtmlListItemCollection : ControlCollection
    {
        public UniHtmlListItemCollection(Control owner)
            : base(owner)
        {
        }
        public void Add(UniHtmlListItem child)
        {
            base.Add(child);
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
            CreateLink();
        }
        public UniHtmlListItem()
        {
        }

        public ControlCollection Items { get { return Controls; } }

        private string iconCssClass;
        string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                CreateLink();
            }
        }

        string navigateUrl;
        public string NavigateUrl
        {
            get { return navigateUrl; }
            set
            {
                navigateUrl = value;
                CreateLink();
            }
        }

        string target;
        public string Target
        {
            get { return target; }
            set
            {
                target = value;
                CreateLink();
            }
        }

        public string Value { get; set; }

        public System.Web.UI.HtmlControls.HtmlAnchor HyperLink { get; private set; }
        public string IconCssClass
        {
            get
            {
                return iconCssClass;
            }
            set
            {
                iconCssClass = value;
                CreateLink();
            }
        }

        void CreateLink()
        {
            if (HyperLink == null)
            {
                HyperLink = new System.Web.UI.HtmlControls.HtmlAnchor();
                Controls.AddAt(0, HyperLink);
            }
            HyperLink.Target = Target ?? "";
            HyperLink.HRef = NavigateUrl ?? "#";
            //HyperLink.InnerText = Text;
            var spanText = (HtmlGenericControl)HyperLink.Controls.Cast<Control>().FirstOrDefault(c => c is HtmlGenericControl
                && ((HtmlGenericControl)c).Attributes["class"].Contains("menuText"));
            if (spanText == null)
            {
                spanText = new HtmlGenericControl("SPAN");
                spanText.Attributes["class"] = "menuText";
                HyperLink.Controls.Add(spanText);
            }
            spanText.InnerHtml = text;
            if (!string.IsNullOrEmpty(iconCssClass))
            {
                var span = (HtmlGenericControl)HyperLink.Controls.Cast<Control>().FirstOrDefault(c => c is HtmlGenericControl
                    && ((HtmlGenericControl)c).Attributes["class"].Contains("iconSpan"));
                if (span == null)
                {
                    span = new HtmlGenericControl("SPAN");
                    span.Attributes["class"] = "iconSpan " + iconCssClass;
                    HyperLink.Controls.AddAt(0, span);
                }

            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Li;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            //base.Controls.Clear();
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].GetType() == typeof(UniHtmlListItem))
                {
                    UniHtmlList l = new UniHtmlList();
                    Control c = Controls[i];
                    Controls.RemoveAt(i);
                    Controls.AddAt(i, l);
                    l.Controls.Add(c);
                    i++;
                    while (i<Controls.Count && Controls[i].GetType() == typeof(UniHtmlListItem))
                    {
                        c = Controls[i];
                        Controls.RemoveAt(i);
                        l.Controls.Add(c);
                    }
                }
            }
            base.Render(writer);
        }

        //protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        //{
        //    if (!string.IsNullOrEmpty(NavigateUrl) || !string.IsNullOrEmpty(Text))
        //    {
        //        if (!string.IsNullOrEmpty(NavigateUrl) )
        //            writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(NavigateUrl));
        //        if (!string.IsNullOrEmpty(Target))
        //            writer.AddAttribute(HtmlTextWriterAttribute.Target, Target);
        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
        //        RenderText(writer);
        //        writer.RenderEndTag();
        //    }
        //    //else
        //    //    RenderText(writer);
        //    if (Items.Count > 0)
        //    {
        //        //writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Ul);
        //        foreach (var item in Items)
        //            item.Render(writer);
        //        //writer.RenderEndTag();
        //        writer.RenderEndTag();
        //    }
        //}
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
