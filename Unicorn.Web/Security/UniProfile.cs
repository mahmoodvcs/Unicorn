using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Profile;
using Unicorn.Data;

namespace Unicorn.Web.Security
{
    public class UniProfile
    {
        public static string[] PropertyNames { get; private set; }
        public static string[] SecurityProperties { get; private set; }
        public static JcoProfileProperty[] Properties { get; private set; }

        static UniProfile()
        {
            Properties = new JcoProfileProperty[ProfileBase.Properties.Count];
            PropertyNames = new string[ProfileBase.Properties.Count];
            int i = 0;
            List<string> secProps = new List<string>();
            foreach (System.Configuration.SettingsProperty prop in ProfileBase.Properties)
            {
                JcoProfileProperty p = new JcoProfileProperty();
                p.Name = prop.Name;
                p.Type = prop.PropertyType;
                p.DefaultValue = prop.DefaultValue;
                object customData = prop.Attributes["CustomProviderData"];
                if (customData != null)
                {
                    string[] data = customData.ToString().Split(';');
                    foreach (string s in data)
                    {
                        if (s.Trim() == "")
                            continue;
                        string[] pair = s.Split('=');
                        switch (pair[0].ToLower())
                        {
                            case "security":
                                if (pair.Length == 1 || pair[1].ToLower() == "true")
                                {
                                    p.IsSecurity = true;
                                    secProps.Add(p.Name);
                                }
                                break;
                            case "title":
                                p.Title = pair[1];
                                break;
                        }
                    }
                }
                Properties[i] = p;
                PropertyNames[i] = p.Name;
                i++;
            }
            SecurityProperties = secProps.ToArray();
        }

        public static string GetSecurityConditionForTable(string userName, TableInfo tableInfo)
        {
            List<string> conditions = new List<string>();
            bool b = true;
            ProfileBase profile = ProfileBase.Create(userName);
            foreach (var p in Properties)
            {
                if (!p.IsSecurity)
                    continue;
                foreach (ColumnInfo ci in tableInfo.Columns)
                {
                    if (ci.ColumnName != p.Name )// !p.Type.FullName.StartsWith("System.Int"))
                        continue;
                    object val = profile.GetPropertyValue(p.Name);
                    if (val == null)
                        continue;
                    string s;
                    if (p.Type.IsArray)
                    {
                        if (((Array)val).Length == 0)
                            continue;
                        s = string.Join(",", ((Array)val).Cast<object>().Select(o1 => o1.ToString()).ToArray());
                    }
                    else if (val.ToString() == "0")//TODO: Default value
                        continue;
                    else
                        s = val.ToString();
                    //if (b)
                    //    b = false;
                    //else
                    //    s += " AND ";
                    conditions.Add( tableInfo.FullName + "." + ci.ColumnName + "=" + s);
                }
            }
            return string.Join(" AND ", conditions.ToArray());
        }

    }
    public class JcoProfileProperty
    {
        public string Name { get; internal set; }
        public string Title { get; internal set; }
        public bool IsSecurity { get; internal set; }
        public Type Type { get; internal set; }
        public object DefaultValue { get; internal set; }
    }
}
