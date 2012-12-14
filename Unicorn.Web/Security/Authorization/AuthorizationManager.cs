﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Web;
using System.Xml;
using System.Web.Caching;
using Unicorn.Data;

namespace Unicorn.Web.Security.Authorization
{
	public static class AuthorizationManager
	{
		static AuthorizationManager()
		{
			actions = new AuthorizedAction("root");
		}
		private static AuthorizedAction actions;
		public static AuthorizedAction Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		#region RegisterAction
		public static void RegisterAction(string action)
		{
			actions.AddSubAction(action);
		}
		public static void RegisterAction(string action, params string[] subActions)
		{
			AuthorizedAction ac = actions.AddSubAction(action);
			foreach (string s in subActions)
			{
				ac.AddSubAction(s);
			}
		}
		public static void RegisterAction(Type actionsEnumType)
		{
			RegisterAction(actions, actionsEnumType);
		}
		public static void RegisterAction(AuthorizedAction parentAction, Type actionsEnumType)
		{
			if (!actionsEnumType.IsEnum)
				throw new ArgumentException("پارامتر بايد از نوع شمارشي باشد.", "actionsEnumType");
			FieldInfo[] fields = actionsEnumType.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fi in fields)
			{
				object[] attrs = fi.GetCustomAttributes(typeof(TitleAttribute), false);
				AuthorizedAction ac = parentAction.AddSubAction(fi.Name);
				if (attrs.Length > 0)
					ac.Title = ((TitleAttribute)attrs[0]).Title;
			}
		}
		public static void RegisterAction(string parentAction, Type actionsEnumType)
		{
			RegisterAction(actions.AddSubAction(parentAction), actionsEnumType);
		}
		#endregion RegisterAction

		private static SqlConnection GetConnection()
		{
			System.Web.Configuration.MembershipSection membership = Configuration.ConfigUtility.GetSystemWebSectionGroup().Membership;
			string constr = WebConfigurationManager.ConnectionStrings[membership.Providers[membership.DefaultProvider].Parameters["connectionStringName"]].ConnectionString;
			SqlConnection con = new SqlConnection(constr);
			return con;
		}
		private static void ExecuteStoredProcedure(string procName, params SqlParameter[] parameters)
		{
			SqlConnection con = GetConnection();
			SqlCommand cmd = new SqlCommand(procName, con);
			foreach (SqlParameter p in parameters)
			{
				cmd.Parameters.Add(p);
			}
			cmd.CommandType = CommandType.StoredProcedure;
			con.Open();
			cmd.ExecuteNonQuery();
			con.Close();
		}

        //public static bool Authorize(HttpContext httpContext, string userName)
        //{
        //    AuthorizedAction[] userActions;
        //    //string cacheKey = "AuthorizedAction_UserActions_" + userName;
        //    //if (httpContext.Cache[cacheKey] == null)
        //    //{
        //        userActions = AuthorizationManager.GetAllActionsForUser(userName);
        //        //httpContext.Cache.Add(cacheKey, userActions, null
        //        //    , Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
        //    //}
        //    //else
        //    //    userActions = (AuthorizedAction[])httpContext.Cache[cacheKey];
        //    foreach (AuthorizedAction action in userActions)
        //    {
        //        if (actions.HasAnySubAction(action))
        //            return true;
        //    }
        //    return false;
        //}

		#region GetUserActions

        private static string GetCacheKey(string userName)
        {
            string cacheKey = "JcoAuthorization_UserActions_" + userName.ToLower();
            return cacheKey;
        }
        public static string[] GetAllActionsForUser(string userName)
		{
            userName = userName.ToLower();
            string[] userActions;
            string cacheKey = GetCacheKey(userName);
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                List<string> list = new List<string>();
                list.AddRange(GetUserActions(userName));
                string[] roles = Roles.GetRolesForUser(userName);
                foreach (string r in roles)
                {
                    list.AddRange(GetRoleActions(r));
                }
                userActions = list.ToArray();
                HttpContext.Current.Cache.Add(cacheKey, userActions, null
                    , Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), CacheItemPriority.Normal, null);
            }
            else
                userActions = (string[])HttpContext.Current.Cache[cacheKey];
            return userActions;
		}

		public static string[] GetUserActions(string userName)
		{
			return GetActions(userName, true);
		}
		public static string[] GetRoleActions(string roleName)
		{
			return GetActions(roleName, false);
		}
		private static string[] GetActions(string userOrRoleName, bool isUser)
		{
			if (!Configuration.ConfigInitializer.CheckConfig(new System.Web.UI.Control()))
				return new string[0];
			SqlConnection con = GetConnection();
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = con;
			if (isUser)
			{
				cmd.CommandText = "aspnet_Authorization_GetUserActions";
				cmd.Parameters.Add(new SqlParameter("@UserName", userOrRoleName));
			}
			else
			{
				cmd.CommandText = "aspnet_Authorization_GetRoleActions";
				cmd.Parameters.Add(new SqlParameter("@RoleName", userOrRoleName));
			}
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			con.Open();
			SqlDataReader dr = cmd.ExecuteReader();
			List<string> actions = new List<string>();

			while (dr.Read())
			{
				actions.Add((string)dr[0]);
			}
			dr.Close();
			con.Close();
			return actions.ToArray();
		}
		#endregion

		#region Add/Remove Actions
		public static void AddActionForUser(string userName, string action, params string[] subActions)
		{
			if (subActions.Length == 0)
				AddAction(userName, action, true);
			else
				foreach (string s in subActions)
					AddAction(userName, action + "." + s, true);
		}
		public static void AddActionForRole(string roleName, string action, params string[] subActions)
		{
			if (subActions.Length == 0)
				AddAction(roleName, action, false);
			else
				foreach (string s in subActions)
					AddAction(roleName, action + "." + s, false);
		}
		private static void AddAction(string userOrRoleName, string actionText, bool isUser)
		{
			SqlConnection con = GetConnection();
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = con;
			if (isUser)
			{
				cmd.CommandText = "aspnet_Authorization_AddActionForUser";
				cmd.Parameters.Add(new SqlParameter("@UserName", userOrRoleName));
                UserAuthorizationChanged(HttpContext.Current, userOrRoleName);
			}
			else
			{
				cmd.CommandText = "aspnet_Authorization_AddActionForRole";
				cmd.Parameters.Add(new SqlParameter("@RoleName", userOrRoleName));
                RoleAuthorizationChanged(HttpContext.Current, userOrRoleName);
            }
			cmd.Parameters.Add(new SqlParameter("@ActionText", actionText));
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			con.Open();
			cmd.ExecuteNonQuery();
			con.Close();
		}
		public static void ClearUserActions(string userName)
		{
            SqlHelper.ExecuteNonQueryProcedure("aspnet_Authorization_ClearUserActions", new SqlParameter("@UserName", userName));
            AuthorizationManager.UserAuthorizationChanged(userName);
        }
        public static void ClearUserActions(string userName, string actionPrefix)
        {
            string id = SqlHelper.ExecuteScaler("SELECT  UserId FROM dbo.aspnet_Users WHERE LoweredUserName = @user",
                new SqlParameter("@user", userName.ToLower())).ToString().ToUpper();

            SqlHelper.ExecuteNonQuery("delete from aspnet_UserRoleActions where upper(UserRoleId)=@id and left(ActionName, @len) = @prefix"
                , new SqlParameter("@id", id)
                , new SqlParameter("@len", actionPrefix.Length)
                , new SqlParameter("@prefix", actionPrefix));
            AuthorizationManager.UserAuthorizationChanged(userName);
        }
        public static void ClearRoleActions(string roleName, string actionPrefix)
        {
            string id = SqlHelper.ExecuteScaler("SELECT  RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = @role",
                new SqlParameter("@role", roleName.ToLower())).ToString();
            
            SqlHelper.ExecuteNonQuery("delete from aspnet_UserRoleActions where UserRoleId=@id and left(ActionName, @len) = @prefix"
                , new SqlParameter("@id", id)
                , new SqlParameter("@len", actionPrefix.Length)
                , new SqlParameter("@prefix",actionPrefix));
            RoleAuthorizationChanged(roleName);
        }
		public static void ClearRoleActions(string roleName)
		{
            SqlHelper.ExecuteNonQueryProcedure("aspnet_Authorization_ClearRoleActions", new SqlParameter("@RoleName", roleName));
            RoleAuthorizationChanged(roleName);
        }
		#endregion

        public static void UserAuthorizationChanged(string userName)
        {
            UserAuthorizationChanged(HttpContext.Current, userName);
        }
        public static void UserAuthorizationChanged(HttpContext context, string userName)
        {
            //string cacheKey = "AuthorizedAction_UserActions_" + userName;
            context.Cache.Remove(GetCacheKey(userName));
        }
        public static void RoleAuthorizationChanged(string roleName)
        {
            RoleAuthorizationChanged(HttpContext.Current, roleName);
        }
        public static void RoleAuthorizationChanged(HttpContext context, string roleName)
        {
            foreach (string u in Roles.GetUsersInRole(roleName))
            {
                UserAuthorizationChanged(context, u);
            }
        }
        
        public static void AddActionsFromSiteMap()
		{
			HttpContext context = HttpContext.Current;
			XmlDocument doc = new XmlDocument();
			//Page p = (Page)type.GetProperty("Page").GetValue(menu, null);
			doc.Load(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
            AuthorizedAction ac = new AuthorizedAction("Menu");
            ac.Title = "منو";
            actions.SubActions.Add(ac);
			AddActionsFromSiteMap(doc.DocumentElement.FirstChild.ChildNodes, ac);

		}
		private static void AddActionsFromSiteMap(XmlNodeList nodes, AuthorizedAction actions)
		{
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Comment)
					continue;
				string name = "";
				if (node.Attributes["action"] != null)
					name = node.Attributes["action"].Value;
				else if (node.Attributes["url"] != null)
				{
					name = node.Attributes["url"].Value;
					if (name.ToLower().EndsWith(".aspx"))
						name = name.Remove(name.Length - 4);
					//name = name.Substring(name.LastIndexOf('\\') + 1);
				}
                //if (!name.ToLower().StartsWith("menu."))
                //    name = "Menu." + name;
                name = name.Replace('.', '_');
				AuthorizedAction ac = new AuthorizedAction(name);
				if (node.Attributes["title"] != null)
					ac.Title = node.Attributes["title"].Value;
				else
					ac.Title = name;
				actions.SubActions.Add(ac);
				if (node.HasChildNodes)
					AddActionsFromSiteMap(node.ChildNodes, ac);
			}
		}
	}
}
