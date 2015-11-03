using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Unicorn.Mvc
{
    public static class AngularHelpers
    {
        public static string AppVersion { get; private set; }
        class AngularViewInfo
        {
            public string Name { get; set; }
            public string ScriptPath { get; set; }
            public bool HasScript { get; set; }
            public int DuplicateIndex { get; set; }
            public string UniqueName 
            {
                get
                {
                    if (DuplicateIndex > 0)
                        return Name + DuplicateIndex.ToString();
                    return Name;
                }
            }
        }

        public static MvcHtmlString AngularView(this HtmlHelper html, string viewName, string scriptPath = null, bool hasScript = true)
        {
            List<AngularViewInfo> views = (List<AngularViewInfo>)(html.ViewContext.TempData["angularViews"] ?? new List<AngularViewInfo>());
            html.ViewContext.TempData["angularViews"] = views;
            var dupIndex = views.Where(v => v.Name == viewName).Max(v => (int?)v.DuplicateIndex);

            var view = new AngularViewInfo()
            {
                Name = viewName,
                ScriptPath = scriptPath,
                HasScript = hasScript
            };
            views.Add(view);
            if (dupIndex > 0)
                view.DuplicateIndex = dupIndex.Value + 1;
            //var s = ReadFile("~/app/" + viewName + ".html");
            //return new MvcHtmlString(s);
            return new MvcHtmlString("<div class='angularViewPlace' data-view='" + viewName + "'></div>");
        }
        public static void RenderAngularView(this HtmlHelper html, string viewName, string scriptPath = null, bool hasScript = true)
        {
            html.ViewContext.HttpContext.Response.Write(AngularView(html, viewName, scriptPath, hasScript));
        }
        public static MvcHtmlString AngularScripts(this HtmlHelper html, string basePath = "~/app/")
        {
            if (AppVersion == null)
                AppVersion = System.IO.File.GetLastWriteTime(Assembly.GetCallingAssembly().Location).Ticks.ToString();
            List<AngularViewInfo> views = (List<AngularViewInfo>)html.ViewContext.TempData["angularViews"];
            var url = new UrlHelper(html.ViewContext.RequestContext);
            StringBuilder sb = new StringBuilder();
            sb.Append("<script>").Append(Environment.NewLine)
                .Append("var Unicorn_AppVersion = ").Append(AppVersion).Append(";").Append(Environment.NewLine)
                .Append("var baseUrl= '").Append(url.Content("~")).Append("';").Append(Environment.NewLine)
                .Append(" </script>");
            if (views != null)
            {
                sb.Append("<script src='")
                    .Append(url.Content("~/Unicorn_Resource?a=Unicorn.Mvc&r=Unicorn.Mvc.js.angularHelper.js&"))
                    .Append(File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).Ticks.ToString())
                    .Append("'></script>\r\n");

                if (!basePath.EndsWith("/"))
                    basePath += "/";
                foreach (var v in views)
                {
                    if (v.HasScript)
                        sb.Append("<script src='").Append(url.Content(basePath + (v.ScriptPath ?? v.Name) + ".js?"))
                            .Append(AppVersion)
                            .Append("'></script>\r\n");
                }
            }
            return new MvcHtmlString(sb.ToString());
        }
        public static void RenderAngulatScripts(this HtmlHelper html)
        {
            html.ViewContext.HttpContext.Response.Write(AngularScripts(html));
        }
        static string GetAppPath()
        {
            return HttpContext.Current.Request.ApplicationPath == "/"
                ? string.Empty
                : HttpContext.Current.Request.ApplicationPath;
        }
        private static void RenderFile(HtmlHelper html, string path)
        {
            html.ViewContext.HttpContext.Response.Write(ReadFile(path));
        }
        static string ReadFile(string path)
        {
            return File.ReadAllText(HttpContext.Current.Server.MapPath(path)).Replace("{appPath}", GetAppPath());
        }
    }
}
