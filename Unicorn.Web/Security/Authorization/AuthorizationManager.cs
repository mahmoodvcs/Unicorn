using System;
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
using System.IO;
using Common.Logging;
using System.Runtime.Caching;
using System.Linq;

namespace Unicorn.Web.Security.Authorization
{
    public static class AuthorizationManager
    {
        static ILog log;
        static AuthorizationManager()
        {
            actions = new AuthorizedAction("root");
            log = new Common.Logging.Simple.NoOpLogger(); //LogManager.GetCurrentClassLogger();
            var t = SqlHelper.ExecuteScaler(@"SELECT lower(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'aspnet_users'");
            if (t is string && (string)t == "aspnet_users")
            {
                rolesTableName = "aspnet_roles";
                usersTableName = "aspnet_users";
                //userRolesTabeName = "aspnet_UsersInRoles";
            }
            else
            {
                rolesTableName = "roles";
                usersTableName = "users";
                //userRolesTabeName = "UsersInRoles";
            }
            userRoleActionsTableName = System.Web.Configuration.WebConfigurationManager.AppSettings["Uniconr.UserRoleActionsTableName"];
            if (userRoleActionsTableName == null)
                userRoleActionsTableName = "aspnet_UserRoleActions";
            if (SqlHelper.ExecuteScaler(@"SELECT * FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = '" + userRoleActionsTableName + "'") == null)
            {
                ExecuteScalar(@"CREATE TABLE [dbo].[" + userRoleActionsTableName + @"](
	[ActionName] [nvarchar](max) NOT NULL,
	[UserRoleId] [uniqueidentifier] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
");
            }
        }
        internal static readonly string rolesTableName, usersTableName//, userRolesTabeName, 
            , userRoleActionsTableName;
        private static AuthorizedAction actions;
        public static AuthorizedAction Actions
        {
            get { return actions; }
            set { actions = value; }
        }

        static MemoryCache _authorizationCache = null;
        static MemoryCache AuthorizationCache
        {
            get
            {
                if (_authorizationCache == null)
                {
                    _authorizationCache = new MemoryCache("AuthorizationCache");
                }
                return _authorizationCache;
            }
        }

        #region RegisterAction
        public static AuthorizedAction RegisterAction(string action)
        {
            return actions.AddSubAction(action);
        }
        public static AuthorizedAction RegisterAction(string action, params string[] subActions)
        {
            AuthorizedAction ac = actions.AddSubAction(action);
            foreach (string s in subActions)
            {
                ac.AddSubAction(s);
            }
            return ac;
        }
        public static AuthorizedAction RegisterAction<EnumType>(string parentAction, bool onlyAddChilds = false)
        {
            return RegisterAction(parentAction, typeof(EnumType), onlyAddChilds);
        }
        public static AuthorizedAction RegisterAction<EnumType>(bool onlyAddChilds = false)
        {
            return RegisterAction(typeof(EnumType), onlyAddChilds);
        }
        public static AuthorizedAction RegisterAction<EnumType>(AuthorizedAction parentAction, bool onlyAddChilds = false)
        {
            return RegisterAction(parentAction, typeof(EnumType), onlyAddChilds);
        }
        public static AuthorizedAction RegisterAction(Type actionsEnumType, bool onlyAddChilds = false)
        {
            //AuthorizedAction ac = new AuthorizedAction(actionsEnumType.Name);
            //ac.Title = GetTitle(actionsEnumType, actionsEnumType.Name, actionsEnumType);
            //actions.SubActions.Add(ac);
            return RegisterAction(actions, actionsEnumType, onlyAddChilds);
            //return ac;
        }
        public static AuthorizedAction RegisterAction(string parentAction, Type actionsEnumType, bool onlyAddChilds = false)
        {
            var ac = actions[parentAction];
            if (ac == null)
                ac = actions.AddSubAction(parentAction);
            RegisterAction(ac, actionsEnumType, onlyAddChilds);
            return ac;
        }
        public static AuthorizedAction GetEnumActions<EnumType>(AuthorizedAction addChildsToThis = null)
        {
            var t = typeof(EnumType);
            AuthorizedAction enumAction;
            if (addChildsToThis == null)
            {
                enumAction = new AuthorizedAction(t.Name);
                enumAction.Title = GetTitle(t, t.Name);
            }
            else
                enumAction = addChildsToThis;
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                if (fi.Name.Contains("_"))
                {
                    var acs = fi.Name.Split('_');
                    string title = GetTitle(fi, fi.Name);
                    var tis = title.Split('.');
                    var ac = enumAction;
                    for (int i = 0; i < acs.Length; i++)
                    {
                        ac = ac.AddSubAction(acs[i]);
                        ac.Title = tis[i];
                    }
                }
                else
                {
                    AuthorizedAction ac = enumAction.AddSubAction(fi.Name);
                    ac.Title = GetTitle(fi, fi.Name);
                }
            }
            return enumAction;

        }
        //TODO: remove duplicate code: GetEnumActions
        public static AuthorizedAction RegisterAction(AuthorizedAction parentAction, Type actionsEnumType, bool onlyAddChilds = false)
        {
            if (!actionsEnumType.IsEnum)
                throw new ArgumentException("پارامتر بايد از نوع شمارشي باشد.", "actionsEnumType");
            AuthorizedAction enumAction;
            if (onlyAddChilds)
                enumAction = parentAction;
            else
            {
                enumAction = new AuthorizedAction(actionsEnumType.Name);
                enumAction.Title = GetTitle(actionsEnumType, enumAction.Name);
                parentAction.SubActions.Add(enumAction);
            }

            FieldInfo[] fields = actionsEnumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                if (fi.Name.Contains("_"))
                {
                    var acs = fi.Name.Split('_');
                    string title = GetTitle(fi, fi.Name);
                    var tis = title.Split('.');
                    var ac = enumAction;
                    for (int i = 0; i < acs.Length; i++)
                    {
                        ac = ac.AddSubAction(acs[i]);
                        ac.Title = tis[i];
                    }
                }
                else
                {
                    AuthorizedAction ac = enumAction.AddSubAction(fi.Name);
                    ac.Title = GetTitle(fi, fi.Name);
                }
            }
            return enumAction;
        }
        static string GetTitle(ICustomAttributeProvider typeMember, string name = null, Type containingType = null)
        {
            return TitleAttribute.GetTitle(typeMember, name, containingType);
        }
        //public static void RegisterAction(string parentAction, Type actionsEnumType)
        //{
        //    RegisterAction(actions.AddSubAction(parentAction), actionsEnumType);
        //}
        #endregion RegisterAction

        static string _connectionString;
        private static SqlConnection GetConnection()
        {
            if (_connectionString == null)
            {
                System.Web.Configuration.MembershipSection membership = Configuration.ConfigUtility.GetSystemWebSectionGroup().Membership;
                _connectionString = WebConfigurationManager.ConnectionStrings[membership.Providers[membership.DefaultProvider].Parameters["connectionStringName"]].ConnectionString;
            }
            SqlConnection con = new SqlConnection(_connectionString);
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
        private static SqlDataReader ExecuteReader(string command, params SqlParameter[] parameters)
        {
            SqlConnection con = GetConnection();
            SqlCommand cmd = new SqlCommand(command, con);
            foreach (SqlParameter p in parameters)
                cmd.Parameters.Add(p);
            con.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        private static object ExecuteScalar(string command, params SqlParameter[] parameters)
        {
            SqlConnection con = GetConnection();
            SqlCommand cmd = new SqlCommand(command, con);
            foreach (SqlParameter p in parameters)
                cmd.Parameters.Add(p);
            con.Open();
            var v = cmd.ExecuteScalar();
            con.Close();
            return v;
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
            string cacheKey = "UnicornAuthorization_UserActions_" + userName.ToLower();
            return cacheKey;
        }
        public static string[] GetAllActionsForUser(string userName)
        {
            userName = userName.ToLower();
            string[] userActions;
            string cacheKey = GetCacheKey(userName);
            if (AuthorizationCache[cacheKey] == null)
            {
                List<string> list = new List<string>();
                list.AddRange(GetUserActions(userName));
                string[] roles = Roles.GetRolesForUser(userName);
                foreach (string r in roles)
                {
                    list.AddRange(GetRoleActions(r));
                }
                log.Trace(m => m("Loading from database. user: '" + userName + "'. Actions:  " + string.Join(", ", list)));
                userActions = list.ToArray();
                AuthorizationCache.Add(cacheKey, userActions, new CacheItemPolicy()
                {
                    SlidingExpiration = new TimeSpan(2, 0, 0)
                });
            }
            else
            {
                userActions = (string[])AuthorizationCache[cacheKey];
                log.Trace("Returning cahced. user: " + userName);
            }
            return userActions;
        }

        public static string[] GetUserActions(string userName, string parentAction = null)
        {
            return GetActions(userName, true, parentAction);
        }
        public static string[] GetRoleActions(string roleName, string parentAction = null)
        {
            return GetActions(roleName, false, parentAction);
        }
        private static string[] GetActions(string userOrRoleName, bool isUser, string parentAction = null)
        {
            string userRoleId = GetUserRoleId(userOrRoleName, isUser);
            if (userRoleId == null)
                return new string[0];
            return GetActions(userRoleId, parentAction);
        }
        public static string[] GetActions(string userOrRoleId, string parentAction = null)
        {
            var cmd = string.Format("select ActionName from {0} where UserRoleId='{1}'", userRoleActionsTableName, userOrRoleId);
            List<SqlParameter> pars= new List<SqlParameter>();
            if (parentAction != null)
            {
                cmd += $" and ActionName like @p + '.%'";
                pars.Add(new SqlParameter("@p", parentAction));
            }
            var dr = ExecuteReader(cmd, pars.ToArray());

            //SqlConnection con = GetConnection();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            //if (isUser)
            //{
            //    cmd.CommandText = "aspnet_Authorization_GetUserActions";
            //    cmd.Parameters.Add(new SqlParameter("@UserName", userOrRoleName));
            //}
            //else
            //{
            //    cmd.CommandText = "aspnet_Authorization_GetRoleActions";
            //    cmd.Parameters.Add(new SqlParameter("@RoleName", userOrRoleName));
            //}
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //con.Open();
            //SqlDataReader dr = cmd.ExecuteReader();
            List<string> actions = new List<string>();

            while (dr.Read())
            {
                actions.Add((string)dr[0]);
            }
            dr.Close();
            //con.Close();
            return actions.ToArray();
        }

        public static string GetUserRoleId(string userOrRoleName, bool isUser)
        {
            string userRoleId;
            object o;
            if (isUser)
                o = ExecuteScalar(string.Format("select userid from {0} where lower(username)=@un", usersTableName), new SqlParameter("@un", userOrRoleName.ToLower()));
            else
                o = ExecuteScalar(string.Format("select roleid from {0} where lower(rolename)=@un", rolesTableName), new SqlParameter("@un", userOrRoleName.ToLower()));
            if (o == null)
                return null;
            userRoleId = o.ToString();
            return userRoleId;
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
        public static void AddAction(string userOrRoleName, string actionText, bool isUser)
        {
            string userRoleId = GetUserRoleId(userOrRoleName, isUser);
            AddAction(userRoleId, actionText);
            if (isUser)
            {
                log.Trace(m => m("User: '" + userOrRoleName + "' - Action: '" + actionText + "'"));
                UserAuthorizationChanged(HttpContext.Current, userOrRoleName);
            }
            else
            {
                log.Trace(m => m("Role: '" + userOrRoleName + "' - Action: '" + actionText + "'"));
                RoleAuthorizationChanged(HttpContext.Current, userOrRoleName);
            }

        }
        public static void AddAction(string userOrRoleId, string action)
        {
            //SqlConnection con = GetConnection();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            ExecuteScalar("insert into " + userRoleActionsTableName + " (userRoleId, ActionName) values (@uid, @ac)",
                new SqlParameter("@uid", userOrRoleId), new SqlParameter("@ac", action));
            //cmd.Parameters.Add(new SqlParameter("@ActionText", actionText));
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //con.Open();
            //cmd.ExecuteNonQuery();
            //con.Close();
        }
        public static void ClearUserActions(string userName)
        {
            string userRoleId = GetUserRoleId(userName, true);
            ExecuteScalar("delete " + userRoleActionsTableName + " where userroleid=@uid",
                new SqlParameter("@uid", userRoleId));
            //SqlHelper.ExecuteNonQueryProcedure("aspnet_Authorization_ClearUserActions", new SqlParameter("@UserName", userName));
            AuthorizationManager.UserAuthorizationChanged(userName);
        }
        public static void ClearUserActions(string userName, string actionPrefix)
        {
            string userRoleId = GetUserRoleId(userName, true);
            //var user = Membership.GetUser(userName);
            //var o = user.ProviderUserKey;
            //if (o == null || o == DBNull.Value)
            //    return;

            //string id = o.ToString();
            string where = GetActionPrefixCondition(actionPrefix);
            ExecuteScalar("delete from " + userRoleActionsTableName + " where upper(UserRoleId)=@id and " + where
                , new SqlParameter("@id", userRoleId)
                , new SqlParameter("@len", actionPrefix.Length)
                , new SqlParameter("@prefix", actionPrefix));
            AuthorizationManager.UserAuthorizationChanged(userName);
        }
        static string GetActionPrefixCondition(string actionPrefix, bool revokeAccessToSubActions = true)
        {
            string where;
            if (actionPrefix.EndsWith("."))
                where = "left(ActionName, @len) = @prefix";
            else if(revokeAccessToSubActions)
                where = "(left(ActionName, @len+1) = (@prefix+'.') or ActionName = @prefix)";
            else
                where = "(ActionName = @prefix)";
            return where;
        }
        public static void ClearRoleActions(string roleName, string actionPrefix)
        {
            string userRoleId = GetUserRoleId(roleName, false);
            //object o;
            //try
            //{
            //    o = SqlHelper.ExecuteScaler("SELECT RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = @role",
            //        new SqlParameter("@role", roleName.ToLower()));
            //}
            //catch (SqlException)
            //{
            //    o = SqlHelper.ExecuteScaler("SELECT RoleId FROM dbo.Roles WHERE Lower(RoleName) = @role",
            //        new SqlParameter("@role", roleName.ToLower()));
            //}
            //if (o == null || o == DBNull.Value)
            //    return;
            //string id = o.ToString();

            string where = GetActionPrefixCondition(actionPrefix);
            SqlHelper.ExecuteNonQuery("delete from " + userRoleActionsTableName + " where UserRoleId=@id and " + where
                , new SqlParameter("@id", userRoleId)
                , new SqlParameter("@len", actionPrefix.Length)
                , new SqlParameter("@prefix", actionPrefix));
            RoleAuthorizationChanged(roleName);
        }
        public static void ClearRoleActions(string roleName)
        {
            string userRoleId = GetUserRoleId(roleName, false);
            ExecuteScalar("delete " + userRoleActionsTableName + " where userroleid=@uid",
                new SqlParameter("@uid", userRoleId));
            //SqlHelper.ExecuteNonQueryProcedure("aspnet_Authorization_ClearRoleActions", new SqlParameter("@RoleName", roleName));
            RoleAuthorizationChanged(roleName);
        }
        public static void RevokeAccess(string userOrRoleId, string action, bool revokeAccessToSubActions = true)
        {
            var cmd = $"delete from {userRoleActionsTableName} where upper(UserRoleId)=@id";
            if (!string.IsNullOrEmpty(action))
            {
                string where = GetActionPrefixCondition(action, revokeAccessToSubActions);
                cmd += " and " + where;
            }
            ExecuteScalar(cmd
                , new SqlParameter("@id", userOrRoleId)
                , new SqlParameter("@len", action.Length)
                , new SqlParameter("@prefix", action));
        }
        public static void ClearAuthorizationCache()
        {
            AuthorizationCache.Dispose();
            _authorizationCache = null;
        }
        #endregion

        public static void UserAuthorizationChanged(string userName)
        {
            UserAuthorizationChanged(HttpContext.Current, userName);
        }
        public static void UserAuthorizationChanged(HttpContext context, string userName)
        {
            //string cacheKey = "AuthorizedAction_UserActions_" + userName;
            log.Trace(m => m("User: '" + userName + "'"));
            AuthorizationCache.Remove(GetCacheKey(userName));
        }
        public static void RoleAuthorizationChanged(string roleName)
        {
            RoleAuthorizationChanged(HttpContext.Current, roleName);
        }
        public static void RoleAuthorizationChanged(HttpContext context, string roleName)
        {
            log.Trace(m => m("Role: '" + roleName + "'"));
            foreach (string u in System.Web.Security.Roles.GetUsersInRole(roleName))
            {
                UserAuthorizationChanged(context, u);
            }
        }

        public static AuthorizedAction GetLocalizedAction(Func<string, string> localizer)//, bool filterUserAccess = true)
        {
            var actions = (AuthorizedAction)AuthorizationManager.actions.Clone();
            //FilterUserAccess(actions);
            LocalizeAction(actions, localizer);
            return actions;
        }

        //private static void FilterUserAccess(string parentAction, AuthorizedAction actions)
        //{
        //    for (int i = 0; i < actions.SubActions.Count; i++)
        //    {
        //        if(AuthorizationChecker.HasAccess(actions.SubActions[i].FullName, true))

        //    }
        //}

        public static void LocalizeAction(AuthorizedAction action, Func<string, string> localizer)
        {
            if (!string.IsNullOrEmpty(action.Title))
                action.Title = localizer(action.Title);
            foreach (var ac in action.SubActions)
                LocalizeAction(ac, localizer);
        }

        public static void RegisterSiteMapActions()
        {
            AuthorizedAction ac = new AuthorizedAction("Menu");
            ac.Title = "منو";
            actions.SubActions.Add(ac);
            AddActionsFromSiteMap(ac);
        }
        public static void AddActionsFromSiteMap(AuthorizedAction actions)
        {
            actions.SubActions.Clear();
            //HttpContext context = HttpContext.Current;
            XmlDocument doc = new XmlDocument();
            //Page p = (Page)type.GetProperty("Page").GetValue(menu, null);
            doc.Load(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
            AddActionsFromSiteMap(doc.DocumentElement.FirstChild.ChildNodes, actions);
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
                        name = name.Remove(name.Length - 5);
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
        public static void ClearRegisteredActions()
        {
            actions.SubActions.Clear();
        }

        public static AuthorizedAction FindAction(string action)
        {
            if (string.IsNullOrEmpty(action))
                return actions;
            return FindAction(actions, action);
        }
        public static AuthorizedAction FindAction(AuthorizedAction parentAction, string subaction)
        {
            var i = subaction.IndexOf(".");
            var action = subaction;
            if (i > 0)
            {
                action = subaction.Substring(0, i);
                subaction = subaction.Substring(i + 1);
            }
            else
                subaction = null;
            var ac = parentAction.SubActions.FirstOrDefault(a => a.Name == action);
            if (subaction != null)
                return FindAction(ac, subaction);
            else
                return ac;
        }

    }
}
