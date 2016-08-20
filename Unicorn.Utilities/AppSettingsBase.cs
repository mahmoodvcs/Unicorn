using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Utilities
{
    public abstract class AppSettingsBase
    {
        public static string Get([CallerMemberName]string name = null)
        {
            return Get<string>(name);
        }
        public static T Get<T>([CallerMemberName]string name = null)
        {
            return Get<T>(default(T), name);
        }
        public static T Get<T>(T defaultValue, [CallerMemberName]string name = null)
        {
            string v = ConfigurationManager.AppSettings[name];
            if (v != null)
            {
                if (typeof(T) == typeof(int[]))
                    return (T)(object)v.Split(',').Select(a => int.Parse(a)).ToArray();
                else if (typeof(T) == typeof(float[]))
                    return (T)(object)v.Split(',').Select(a => float.Parse(a)).ToArray();
                return (T)Convert.ChangeType(v, typeof(T));
            }
            return defaultValue;
        }
        //public static string SmtpServer
        //{
        //    get { return Get(); }// return WebConfigurationManager.AppSettings["SmtpServer"]; }
        //}
        //public static string MailAddress
        //{
        //    get { return Get(); }// return WebConfigurationManager.AppSettings["MailAddress"]; }
        //}
        //public static string MailPassword
        //{
        //    get { return Get(); }//return WebConfigurationManager.AppSettings["MailPassword"]; }
        //}
        //public static float MinZoomLevel { get { return Get(5); } }
        //public static float MaxZoomLevel { get { return Get(17); } }
        //public static string BaseMapSource { get { return Get(defaultValue: "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"); } }
        //public static float[] MapBounds { get { return Get(defaultValue: new float[] { 25, 43, 40, 64 }); } }
    }
}
