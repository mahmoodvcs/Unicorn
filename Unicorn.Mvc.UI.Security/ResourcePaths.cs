using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Unicorn.Mvc;

namespace Unicorn.Mvc.UI
{
    static class ResourcePaths
    {

        public static readonly string NameSpace = $"{nameof(Unicorn)}.{nameof(Mvc)}.{nameof(UI)}";
        public static readonly string EditableGridPath = $"{NameSpace}.app.controls.data.editableGrid.js";
        public static readonly string AccessTreePath = $"{NameSpace}.app.controls.security.accessTree.js";
        public static readonly string CheckBoxTreeJsPath = $"{NameSpace}.lib.react_checkbox_tree.react-checkbox-tree.js";
        public static readonly string CheckBoxTreeCssPath = $"{NameSpace}.lib.react_checkbox_tree.react-checkbox-tree.css";
        public static string GetResourceContentPath(string path, HtmlHelper html = null)
        {
            UrlHelper url;
            if (html == null)
                url = MVCHelpers.CreateUrlHelper();
            else
                url = new UrlHelper(html.ViewContext.RequestContext);
            return url.Content($"~/Unicorn_Resource?a={NameSpace}&r={path.Replace('/', '.')}");
        }
    }
}
