using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Web.Security;
using System.Xml;
using System.Web;
using System.IO;
using System.Drawing;
using Unicorn.Web;

namespace Unicorn.Web.Security.Authorization
{
    public abstract class AuthorizationPage : Page
    {
        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            authorized = Authorize();
        }
        protected bool authorized = false;
        protected virtual bool Authorize()
        {
            if (IsPostBack)
                return true;
            bool hasAccess = false;
            bool hasAttr = false;
            foreach (object at in GetType().GetCustomAttributes(true))
            {
                IAuthorize auth = at as IAuthorize;
                if (auth != null)
                {
                    hasAttr = true;
                    if (auth.Authorize(this.Context))
                        hasAccess = true;
                }
            }

            if (hasAccess)
                return true;

            if (!hasAttr)
            {
                string sitmap = HttpContext.Current.Server.MapPath("~/Web.sitemap");
                if (File.Exists(sitmap))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(sitmap);

                    foreach (XmlNode childNode in doc.DocumentElement.FirstChild.ChildNodes)
                    {
                        if (childNode.NodeType == XmlNodeType.Comment)
                            continue;
                        if (FindNodeAndCheckAccess(childNode, "Menu", false))
                            return true;
                    }
                }
            }
            FormsAuthentication.RedirectToLoginPage();
            return false;
        }
        public string GetPageUrl()
        {
            return Request.AppRelativeCurrentExecutionFilePath.Remove(0, 2).ToLower();
        }
        private bool FindNodeAndCheckAccess(XmlNode node, string parentAction, bool parentAccess)
        {
            string action = parentAction;
            bool temp;
            bool hasAccess = parentAccess || Utility.HasSiteMapNodeAccess(parentAction, Roles.GetRolesForUser(), User, node, ref action, out temp);
            XmlAttribute urlAttr = node.Attributes["url"];
            string pageUrl = GetPageUrl();
            if (urlAttr != null)
            {
                string url = urlAttr.Value;
                if (url.StartsWith("/"))
                    url = url.Remove(0, 1);
                else if (url.StartsWith("~/"))
                    url = url.Remove(0, 2);
                if (url.IndexOf('?')>-1)
                    url = url.Remove(url.IndexOf('?'));
                if (url.Trim() != "" && pageUrl == url.ToLower())
                    return hasAccess;
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Comment)
                    continue;
                if (FindNodeAndCheckAccess(childNode, action, hasAccess))
                    return true;
            }
            return false;
        }
    }
}
