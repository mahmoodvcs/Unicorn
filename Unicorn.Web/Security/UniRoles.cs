using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unicorn.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Web.Security;
using System.Data.Common;

namespace Unicorn.Web.Security
{
    public static class UniRoles
    {
        public static string GetRoleTitle(string roleName)
        {
            var dr = SqlHelper.ExecuteReader("select * from aspnet_Roles where LoweredRoleName=@r", SqlHelper.CreateParameter("@r", roleName));
            bool b = dr.Read();
            if (!b)
            {
                dr.Close();
                return "";
            }

            int i;
            try
            {
                i = dr.GetOrdinal("Title");
            }
            catch (IndexOutOfRangeException)
            {
                dr.Close();
                AddTitleColumn();
                return roleName;
            }
            string s = dr.GetString2(i);
            dr.Close();
            return string.IsNullOrEmpty(s) ? roleName : s;
        }
        private static void AddTitleColumn()
        {
            try
            {
                SqlHelper.ExecuteNonQuery("alter table aspnet_Roles add Title nvarchar(256) NULL");
            }
            catch (Exception ex)
            {
                throw new Exception("Column 'Title' does not exit in table 'aspnet_Roles' and cannot be created.", ex);
            }
        }

        public static void SetRoleTitle(string roleName, string title)
        {
            try
            {
                SqlHelper.ExecuteNonQuery("update aspnet_Roles set Title=@t where LoweredRoleName=@r",
                    SqlHelper.CreateParameter("@t", title),
                    SqlHelper.CreateParameter("@r", roleName));
            }
            catch (SqlException ex)
            {
                if (ex.Message.IndexOf("Invalid column name 'Title'") >= 0)
                {
                    AddTitleColumn();
                    SetRoleTitle(roleName, title);
                }
                else
                    throw ex;
            }
        }
        public static UniRole[] GetAllRoles()
        {
            List<UniRole> roles = new List<UniRole>();
            DataReader dr = null;
            try
            {
                dr = SqlHelper.ExecuteReader("select RoleName, title from aspnet_Roles");
                NameValueCollection col = new NameValueCollection();
                while (dr.Read())
                {
                    string title = dr.GetString2(1);
                    string name = (string)dr[0];
                    roles.Add(new UniRole() { RoleName = name, RoleTitle = title == "" ? name : title });
                }
                return roles.ToArray();
            }
            catch (SqlException ex)
            {
                if (ex.Message.IndexOf("Invalid column name 'Title'") >= 0)
                {
                    AddTitleColumn();
                    return GetAllRoles();
                }
                else
                    throw ex;
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }
        public static void CreateRole(string roleName, string title)
        {
            Roles.CreateRole(roleName);
            SetRoleTitle(roleName, title);
        }

    }
}
