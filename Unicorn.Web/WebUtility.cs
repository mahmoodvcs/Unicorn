using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.IO;

namespace Unicorn.Web
{
    public partial class WebUtility
    {
        public static void ShowMessageBox(string msg, Page page)
        {
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "msg", "alert('" + EscapeString(msg) + "');", true);
        }

        public static void AddConfirmDialog(string msg, WebControl control)
        {
            control.Attributes["onclick"] = "return confirm('" + msg + "');";
        }

        public static void OpenNewBrowserWindow(Page page, string url)
        {
            OpenNewBrowserWindow(page, url, -1, -1, -1, -1, true, true, true, true, true, true);
        }

        public static void OpenNewBrowserWindow(Page page, string url, int height,
            int width, bool showToolbar, bool showAdressbar,
            bool showMenubar, bool showStatusbar, bool showScrollbars, bool resizable)
        {
            OpenNewBrowserWindow(page, url, height, width, -1, -1, showToolbar,
                showAdressbar, showMenubar, showStatusbar, showScrollbars, resizable);
        }

        public static void OpenNewBrowserWindow(Page page, string url, int height,
            int width)
        {
            OpenNewBrowserWindow(page, url, height, width, -1, -1, true,
                true, true, true, true, true);
        }

        public static void OpenNewBrowserWindow(Page page, string url, int height,
            int width, int top, int left, bool showToolbar, bool showAdressbar,
            bool showMenubar, bool showStatusbar, bool showScrollbars, bool resizable)
        {
            string script = "window.open(\"" + url +
                "\", \"_blank\", \"";
            string features = "";
            if (height > 0)
            {
                if (features != "")
                    features += ",";
                features += "height=" + height.ToString();
            }
            if (width > 0)
            {
                if (features != "")
                    features += ",";
                features += "width=" + width.ToString();
            }
            if (showToolbar)
            {
                if (features != "")
                    features += ",";
                features += "toolbar";
            }
            if (showAdressbar)
            {
                if (features != "")
                    features += ",";
                features += "location";
            }
            if (showMenubar)
            {
                if (features != "")
                    features += ",";
                features += "menubar";
            }
            if (showStatusbar)
            {
                if (features != "")
                    features += ",";
                features += "status";
            }
            if (showScrollbars)
            {
                if (features != "")
                    features += ",";
                features += "scrollbars";
            }
            if (resizable)
            {
                if (features != "")
                    features += ",";
                features += "resizable";
            }
            if (top >= 0)
            {
                if (features != "")
                    features += ",";
                features += "top=" + top.ToString();
            }
            if (left >= 0)
            {
                if (features != "")
                    features += ",";
                features += "left=" + left.ToString();
            }

            script += features + "\");";
            page.ClientScript.RegisterClientScriptBlock(typeof(WebUtility), "OpenBrowseWindow" + url, script, true);
        }

        public static string AddQueryString(string url, string queryStringFragment)
        {
            int i = url.IndexOf("?");
            if (i > 0 && i < url.Length - 1)
                url += "&" + queryStringFragment;
            else
                url += "?" + queryStringFragment;
            return url;
        }

        public static string RemoveQueryString(string url, string queryStringName)
        {
            int i = url.IndexOf("?" + queryStringName + "=");
            if (i == -1)
                i = url.IndexOf("&" + queryStringName + "=");
            if (i == -1)
                return url;
            int j = url.IndexOf("&", i + queryStringName.Length);
            if (j == -1)
                return url.Substring(0, i);
            return url.Substring(0, i + 1) + url.Substring(j + 1);
        }

        public static void ClientRedirect(Page page, string url)
        {
            string script = "window.location.href = '" + page.ResolveClientUrl(url) + "';";
            page.ClientScript.RegisterStartupScript(page.GetType(), "redirect", script, true);
        }

        /* public static void Redirect2(Page pg, string url)
         {
             string script = "window.location.href = '" + pg.ResolveClientUrl(url) + "';";
             HttpContext.Current.Response.Write(script);
         }*/

        public static string EscapeString(string s)
        {
            return s.Replace(@"\", @"\\").Replace("\r", "\\r").Replace("\n", "\\n")
                .Replace("'", "\\'").Replace("\"", "\\\"");
        }

        public static string ConvertHtmlToText(string source)
        {
            string result;
            result = source.Replace("\r", " ");
            result = result.Replace("\n", " ");
            result = result.Replace("\t", string.Empty);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"( )+", " ");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>).*(</script>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*td([^>])*>", "\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*br( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*li( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*div([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*tr([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*p([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result, @"&nbsp;", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"<", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @">", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result, @"&(.{2,6});", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = result.Replace("\n", "\r");
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\t)", "\t\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\r)", "\t\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\t)", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string breaks = "\r\r\r";
            string tabs = "\t\t\t\t\t";
            for (int index = 0; index < result.Length; index++)
            {
                result = result.Replace(breaks, "\r\r");
                result = result.Replace(tabs, "\t\t\t\t");
                breaks = breaks + "\r";
                tabs = tabs + "\t";
            }
            return result;
        }

        public static string GetFullAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            string apppath = HttpContext.Current.Request.ApplicationPath;
            if (apppath.EndsWith("/"))
                apppath = apppath.Remove(apppath.Length - 1);
            path = path.Replace("~", apppath);
            if (path[0] != '/')
                path = "/" + path;
            return path;
        }

        public static string GetContentTypeFromFileName(string fileName)
        {
            return GetContentType(Path.GetExtension(fileName));
        }
        public static string GetContentType(string extention)
        {
            switch (extention)
            {
                case ".avi":
                    return "video/avi";
                case ".bmp":
                    return "image/bmp";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                case ".mov":
                    return "video/quicktime";
                case ".mp3":
                    return "audio/mpeg3";
                case ".mp4":
                    return "audio/mp4";
                case ".mpeg":
                case ".mpg":
                    return "video/mpeg";
                case ".rtf":
                    return "application/rtf";
                case ".tif":
                case ".tiff":
                    return "image/tiff";
                case ".txt":
                    return "text/plain";
                case ".wav":
                    return "audio/wav";
                case ".xls":
                case ".xlsx":
                    return "application/x-excel";
                case ".zip":
                    return "application/x-compressed";
            }
            return "application/octet-stream";
        }
    }

}
