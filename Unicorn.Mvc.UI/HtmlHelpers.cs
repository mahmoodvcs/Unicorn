using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Unicorn.Mvc.UI
{
    public static class HtmlHelpers
    {

        static string GetUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }
        public static HtmlString RenderAccessesEditor(this HtmlHelper htmlHelper, string userOrRoleId, string hiddenInputId, string rootAction = null)
        {
            return new HtmlString($@"
{RenderResource(htmlHelper, ResourcePaths.CheckBoxTreeJsPath)}
{RenderResource(htmlHelper, ResourcePaths.CheckBoxTreeCssPath)}
{RenderResource(htmlHelper, ResourcePaths.AccessTreePath)}
{RenderReactComponent(htmlHelper, "AccessTree", new
            {
                rootAction,
                userOrRoleId,
                hiddenInputId
            })}
");
        }
        //        public static HtmlString RenderUsersEditor(this HtmlHelper htmlHelper)
        //        {
        //            var id = GetUniqueId();
        //            var url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
        //            return new HtmlString($@"
        //<!-- Users Editor -->
        //<script src='{ResourcePaths.GetResourceContentPath("app.angularHelper.js")}' ></script>
        //<div id='div{id}'></div>
        //<script>
        //    $(function () {{
        //        ReactDOM.render(React.createElement(Unicorn.UsersEditor), $(""#div{id}"")[0]);
        //    }});
        //</script>
        //");
        //        }

        public static HtmlString RenderResource(this HtmlHelper htmlHelper, string resourcePath)
        {
            List<string> rendered;
            if (htmlHelper?.ViewData["Unicorn_RenderedResources"] == null)
            {
                rendered = new List<string>();
                if (htmlHelper != null)
                    htmlHelper.ViewData["Unicorn_RenderedResources"] = rendered;
            }
            else
                rendered = (List<string>)htmlHelper.ViewData["Unicorn_RenderedResources"];
            if (!rendered.Contains(resourcePath))
            {
                rendered.Add(resourcePath);
                if (resourcePath.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                    return new HtmlString($"<link href='{ResourcePaths.GetResourceContentPath(resourcePath, ResourcePaths.NameSpace, htmlHelper)}' rel=\"stylesheet\"></script>\r\n");
                else
                    return new HtmlString($"<script src='{ResourcePaths.GetResourceContentPath(resourcePath, ResourcePaths.NameSpace, htmlHelper)}' ></script>\r\n");
            }
            return null;
        }
        public static HtmlString RenderReactComponent(this HtmlHelper htmlHelper, string componentName, object props)
        {
            var id = GetUniqueId();
            return new HtmlString($@"
<!-- ${componentName} -->
<div id='div{id}'></div>
<script>
    $(function () {{
        ReactDOM.render(React.createElement({componentName}, 
                {Newtonsoft.Json.JsonConvert.SerializeObject(props)})
            , document.getElementById(""div{id}""));
    }});
</script>
");
        }
    }
}
