using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Unicorn
{
    public static class AppConfigSaver
    {
        public static T Load<T>(string configFileName = "config.ini") where T : new()
        {
            STA.Settings.INIFile ini = GetINI(configFileName);
            var t = typeof(T);
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var config = new T();
            foreach (var p in props)
            {
                var method = ini.GetType().GetMethod("GetValue", new Type[] { typeof(string), typeof(string), p.PropertyType });
                p.SetValue(config, method.Invoke(ini, new object[] { "", p.Name, GetDefaultValue(p.PropertyType) }), null);
            }
            return config;
        }

        private static STA.Settings.INIFile GetINI(string configFileName)
        {
            var rootPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            var path = Path.Combine(rootPath, configFileName);

            STA.Settings.INIFile ini = new STA.Settings.INIFile(path);
            return ini;
        }

        public static void Save<T>(T config, string configFileName = "config.ini") where T : new()
        {
            STA.Settings.INIFile ini = GetINI(configFileName);
            var t = typeof(T);
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var p in props)
            {
                var method = ini.GetType().GetMethod("SetValue", new Type[] { typeof(string), typeof(string), p.PropertyType });
                method.Invoke(ini, new object[] { "", p.Name, p.GetValue(config, null) });
            }
        }
        //internal static void Save()   
        //{
        //    STA.Settings.INIFile ini = new STA.Settings.INIFile(path);

        //    ini.SetValue("Paths", "File1", File1);
        //    ini.SetValue("Paths", "File2", File2);
        //    ini.SetValue("Paths", "PreCode", PreCode);

        //    ini.Flush();
        //}
        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
