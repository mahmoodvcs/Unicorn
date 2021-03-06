﻿using System;
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
            var dr = SqlHelper.ExecuteReader("select * from " + Authorization.AuthorizationManager.rolesTableName + " where RoleName=@r", SqlHelper.CreateParameter("@r", roleName));
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
        public static string GetRoleOptions(string roleName)
        {
            var dr = SqlHelper.ExecuteReader("select * from " + Authorization.AuthorizationManager.rolesTableName + " where RoleName=@r", SqlHelper.CreateParameter("@r", roleName));
            bool b = dr.Read();
            if (!b)
            {
                dr.Close();
                return "";
            }

            int i;
            try
            {
                i = dr.GetOrdinal("Options");
            }
            catch (IndexOutOfRangeException)
            {
                dr.Close();
                AddOptionsColumn();
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
                SqlHelper.ExecuteNonQuery("alter table " + Authorization.AuthorizationManager.rolesTableName + " add Title nvarchar(256) NULL");
            }
            catch (Exception ex)
            {
                throw new Exception("Column 'Title' does not exit in table '" + Authorization.AuthorizationManager.rolesTableName + "' and cannot be created.", ex);
            }
        }
        private static void AddOptionsColumn()
        {
            try
            {
                SqlHelper.ExecuteNonQuery("alter table " +Authorization.AuthorizationManager.rolesTableName + " add Options nvarchar(MAX) NULL");
            }
            catch (Exception ex)
            {
                throw new Exception("Column 'Options' does not exit in table '" + Authorization.AuthorizationManager.rolesTableName + "' and cannot be created.", ex);
            }
        }

        public static void SetRoleTitle(string roleName, string title)
        {
            try
            {
                SqlHelper.ExecuteNonQuery("update " + Authorization.AuthorizationManager.rolesTableName + " set Title=@t where RoleName=@r",
                    SqlHelper.CreateParameter("@t", title),
                    SqlHelper.CreateParameter("@r", roleName.ToLower()));
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
        public static void SetRoleOptions(string roleName, string options)
        {
            try
            {
                if (options == null)
                    SqlHelper.ExecuteNonQuery("update " + Authorization.AuthorizationManager.rolesTableName + " set Options=NULL where RoleName=@r",
                        SqlHelper.CreateParameter("@r", roleName.ToLower()));
                else
                    SqlHelper.ExecuteNonQuery("update " + Authorization.AuthorizationManager.rolesTableName + " set Options=@t where RoleName=@r",
                        SqlHelper.CreateParameter("@t", options),
                        SqlHelper.CreateParameter("@r", roleName.ToLower()));
            }
            catch (SqlException ex)
            {
                if (ex.Message.IndexOf("Invalid column name 'Options'") >= 0)
                {
                    AddOptionsColumn();
                    SetRoleOptions(roleName, options);
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
                dr = SqlHelper.ExecuteReader("select RoleName, Title, Options from " + Authorization.AuthorizationManager.rolesTableName);
                NameValueCollection col = new NameValueCollection();
                while (dr.Read())
                {
                    string title = dr.GetString2(1);
                    string name = (string)dr[0];
                    roles.Add(new UniRole()
                    {
                        RoleName = name,
                        RoleTitle = title == "" ? name : title,
                        Options = (string)(dr[2] == DBNull.Value ? null : dr[2])
                    });
                }
                return roles.ToArray();
            }
            catch (UniSqlException ex)
            {
                if (ex.Message.IndexOf("Invalid column name 'Title'") >= 0)
                {
                    AddTitleColumn();
                    return GetAllRoles();
                }
                else if (ex.Message.IndexOf("Invalid column name 'Options'") >= 0)
                {
                    AddOptionsColumn();
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
        public static void CreateRole(string roleName, string title, string options = null)
        {
            System.Web.Security.Roles.CreateRole(roleName);
            SetRoleTitle(roleName, title);
            SetRoleOptions(roleName, options);
        }

    }
}
