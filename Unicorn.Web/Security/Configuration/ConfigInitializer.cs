using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using Unicorn.Web.Security;
using System.Web.Security;
using System.Data.Common;

namespace Unicorn.Web.Security.Configuration
{
    public static class ConfigInitializer
    {
        static ConfigInitializer()
        {
        }
        private static bool initialized = false;
        private static List<Control> initAddedButtons = new List<Control>();

        public static void InitializeSql(string connectionStringName)
        {
            if (initialized)
                return;
            try
            {
                System.Configuration.Configuration conf = ConfigUtility.GetConfigFile();
                SystemWebSectionGroup websec = conf.GetSectionGroup("system.web") as SystemWebSectionGroup;
                WebConfigInitializer.InitWebConfig(conf, false, connectionStringName);
            }
            catch
            {
            }
            if (!DatabaseTablesExist)
            {
                InitSqlDatabase(connectionStringName);
            }
            initialized = true;
        }
        //public static void InitializeAccess(string dbPath)
        //{
        //    if (initialized)
        //        return;
        //    System.Configuration.Configuration conf = ConfigUtility.GetConfigFile();
        //    SystemWebSectionGroup websec = conf.GetSectionGroup("system.web") as SystemWebSectionGroup;
        //    ConnectionStringSettings con = conf.ConnectionStrings.ConnectionStrings[AccessConnectionHelper.ConnectionName];
        //    if (con == null)
        //    {
        //        con = new ConnectionStringSettings(AccessConnectionHelper.ConnectionName, dbPath);
        //        conf.ConnectionStrings.ConnectionStrings.Add(con);
        //    }
        //    else
        //        con.ConnectionString = dbPath;
        //    conf.Save();
        //    WebConfigInitializer.InitWebConfig(conf, true, AccessConnectionHelper.ConnectionName);
        //    if (!File.Exists(HttpContext.Current.Server.MapPath(dbPath)))
        //    {
        //        CreateAccessDatabase(dbPath);
        //    }
        //    initialized = true;
        //}

        //private static void CreateAccessDatabase(string dbPath)
        //{
        //    string path = HttpContext.Current.Server.MapPath(dbPath);
        //    Assembly a = Assembly.GetExecutingAssembly();
        //    Stream stream = a.GetManifestResourceStream("Unicorn.Web.Resources.Data.ASPNetDB.mdb");
        //    if (stream == null)
        //        return;
        //    FileStream fs = new FileStream(path, FileMode.Create);
        //    byte[] bs = new byte[4096];
        //    int br;
        //    while ((br = stream.Read(bs, 0, bs.Length)) > 0)
        //        fs.Write(bs, 0, br);
        //    stream.Close();
        //    fs.Close();
        //}
        public static void InitSqlDatabase(string connStrName)
        {
            if (ConnectionManager.DatabaseType != DatabaseType.SQLServer)
                return;
            Assembly a = Assembly.GetExecutingAssembly();
            Stream stream = a.GetManifestResourceStream("Unicorn.Web.Resources.Data.Membership.sql");
            if (stream == null)
                return;
            StreamReader sr = new StreamReader(stream);
            StringBuilder sb = new StringBuilder();
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                if (s.StartsWith("GO"))
                {
                    //SqlHelper.ExecuteNonQuery(sb.ToString());
                    SqlHelper.ExecuteNonQuery(sb.ToString());
                    sb = new StringBuilder();
                    continue;
                }
                sb.Append("\r\n").Append(s);
            }
            if (sb != null && sb.Length > 0)
            {
                SqlHelper.ExecuteNonQuery(sb.ToString());
            }
            //SqlHelper.ExecuteNonQuery(sb.ToString());
            sr.Close();
        }

        internal static bool CheckConfig(Control control)
        {
            if (initialized)
                return true;
            string error;
            HyperLink lnk = new HyperLink();
            if (CheckAppConfiguration(out error))
            {
                //if (Array.IndexOf<string>(Roles.GetAllRoles(), "admin") == -1 && Array.IndexOf<string>(Roles.GetAllRoles(), "Admin") == -1)
                if (Roles.GetAllRoles().Length == 0)
                    Roles.CreateRole("admin");
                if ((Membership.GetUser("admin") == null || Membership.GetUser("Admin") == null) && Membership.GetAllUsers().Count == 0)
                {
                    lnk.Text = "<br>کاربر Admin ايجاد نشده است<br/>براي ايجاد کاربر اينجا را کليک کنيد.";
                    lnk.NavigateUrl = UniHttpHandler.HandlerPath + "?a=conf&b=" + HttpContext.Current.Request.Path + "&p=2";
                    lnk.ToolTip = "کاربر Admin وجود ندارد";
                    control.Controls.Add(lnk);
                }
                else
                    initialized = true;
                return true;
            }

            System.Configuration.Configuration conf = null;
            try
            {
                conf = ConfigUtility.GetConfigFile();
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
            if (conf != null)
            {
                try
                {
                    foreach (ConfigurationLocation location in conf.Locations)
                    {
                        if (location.Path == UniHttpHandler.HandlerPath)
                        {
                            System.Configuration.Configuration lconf = location.OpenConfiguration();
                            SystemWebSectionGroup lweb = ConfigUtility.GetSystemWebSectionGroup(lconf);
                            lweb.Authorization.Rules.Clear();
                            AuthorizationRule rule = new AuthorizationRule(AuthorizationRuleAction.Allow);
                            rule.Users.Add("*");
                            lweb.Authorization.Rules.Add(rule);
                            lconf.Save();
                        }
                    }
                    WebConfigInitializer.CreateHttpHandler(ConfigUtility.GetSystemWebSectionGroup(conf));
                    conf.Save();
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }
            }
            //LinkButton btn = new LinkButton();
            //btn.Text = "تنظيمات امنيتي سايت اتجام نشده است<br/>براي انجام تنظيمات اينجا را کليک کنيد.";
            //btn.Click += new EventHandler( btnDoConfig_Click );
            //btn.CausesValidation = false;
            //btn.ID = "JcoSecDoConfigButton";
            //control.Controls.Add( btn );
            //control.Page.PreRender += new EventHandler( Page_PreRender );
            //initAddedButtons.Add( btn );

            lnk.Text = "تنظيمات امنيتي سايت اتجام نشده است<br/>براي انجام تنظيمات اينجا را کليک کنيد.";
            lnk.NavigateUrl = UniHttpHandler.HandlerPath + "?a=conf&b=" + HttpContext.Current.Request.Path;
            lnk.ToolTip = error;
            control.Controls.Add(lnk);
            return false;
        }

        static void Page_PreRender(object sender, EventArgs e)
        {
            if (initialized)
                foreach (var btn in initAddedButtons)
                {
                    if (btn.Parent != null)
                        btn.Parent.Controls.Remove(btn);
                }
        }

        //static void btnDoConfig_Click(object sender, EventArgs e)
        //{
        //	Initialize();
        //}

        public static bool CheckAppConfiguration(out string error)
        {
            try
            {
                error = "";
                System.Configuration.Configuration conf = null;
                try
                {
                    conf = ConfigUtility.GetConfigFile();
                }
                catch
                {
                }
                SystemWebSectionGroup websec = ConfigUtility.GetSystemWebSectionGroup(conf);
                if (websec.Authentication.Mode != AuthenticationMode.Forms)
                    error = "Authentication.Mode = " + websec.Authentication.Mode.ToString();
                if (websec.Membership.DefaultProvider != UniMembershipProvider.ProviderName)
                    //&& websec.Membership.DefaultProvider != AccessMembershipProvider.ProviderName)
                    if (websec.Membership.DefaultProvider == null)
                        error = "Membership.DefaultProvider = null";
                    else
                        error = "Membership.DefaultProvider = " + websec.Membership.DefaultProvider;
                if (!Roles.Enabled)
                    error = "Roles is not enabled.";
                if (error != "")
                    return false;
                UniSecuritySectionGroup jcoSec = (UniSecuritySectionGroup)conf.GetSectionGroup("uniSecurity");
                if (jcoSec == null)
                    error = "UniSecuritySectionGroup is not defined.";
                //if (jcoSec.MasterPage.Path == "")
                //	error = "JcoSecuritySectionGroup.MasterPage.Path is empty.";
                if (error != "")
                    return false;
                if (websec.Membership.DefaultProvider == UniMembershipProvider.ProviderName)
                {
                    string conName = websec.Membership.Providers[UniMembershipProvider.ProviderName].Parameters["connectionStringName"];
                    bool b = CheckSqlTablesExists(conf.ConnectionStrings.ConnectionStrings[conName].ConnectionString);
                    if (!b)
                    {
                        error = "Sql tables doen not exists.";
                        return false;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                    //string conName = websec.Membership.Providers[AccessMembershipProvider.ProviderName].Parameters["connectionStringName"];
                    //if (!File.Exists(HttpContext.Current.Server.MapPath(conf.ConnectionStrings.ConnectionStrings[conName].ConnectionString)))
                    //{
                    //    error = "Access DB file: '" + conf.ConnectionStrings.ConnectionStrings[conName].ConnectionString + "' does not exists.";
                    //    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
            return true;
        }
        private static bool CheckSqlTablesExists(string connectionString)
        {
            SqlConnection con = null;
            try
            {
                SqlDataReader dr;
                try
                {
                    con = new SqlConnection(connectionString);
                    con.Open();
                }
                catch
                {
                    return true;
                }
                SqlCommand cmd = new SqlCommand("exec sp_tables 'aspnet_Applications'", con);
                dr = cmd.ExecuteReader();
                bool b = dr.Read();
                dr.Close();
                if (b)
                {
                    cmd.CommandText = "exec sp_tables 'aspnet_Roles'";
                    dr = cmd.ExecuteReader();
                    b = dr.Read();
                    dr.Close();
                }
                return b;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public static bool DatabaseTablesExist
        {
            get
            {
                try
                {
                    var dr = SqlHelper.ExecuteReader("exec sp_tables 'aspnet_Applications'");
                    bool b = dr.Read();
                    dr.Close();
                    if (b)
                    {
                        dr = SqlHelper.ExecuteReader("exec sp_tables 'aspnet_Roles'");
                        b = dr.Read();
                        dr.Close();
                    }
                    return b;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
