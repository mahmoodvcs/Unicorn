using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Unicorn.Data
{
    public static class KnoemaLocalizationHelper
    {
        public static string LocalizeText(string text)
        {
            LoadLocalizer();
            if (localizationMethod != null && text != null)
                return (string)localizationMethod.Invoke(null, new object[] { text, typeof(KnoemaLocalizationHelper) });
            return text;
        }
        static MethodInfo localizationMethod;
        static bool localizationMethodNotPresent = false;
        static void LoadLocalizer()
        {
            if (!localizationMethodNotPresent && localizationMethod == null)
            {
                try
                {
                    string path = Path.Combine(new FileInfo(Assembly.GetCallingAssembly().Location).DirectoryName, "Knoema.Localization.Core.dll");
                    if (!File.Exists(path))
                        path = HttpContext.Current.Server.MapPath("~/bin/Knoema.Localization.Core.dll");
                    var ass = Assembly.LoadFile(path);
                    var type = ass.GetType("Knoema.Localization.StringExtensions");
                    var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (var m in methods)
                    {
                        if (m.Name == "Resource" && m.GetParameters()[1].ParameterType == typeof(Type))
                        {
                            localizationMethod = m;
                            return;
                        }
                    }
                }
                catch { }
            }
            localizationMethodNotPresent = true;
        }

    }
}
