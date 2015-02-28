using Unicorn.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Unicorn.Web
{
    public static class Roles
    {
        static Roles()
        {
            cache = MemoryCache.Default;
        }
        static ObjectCache cache;
        public static string[] GetRolesForUser(string username)
        {
            var item = cache.Get(GetUserRoleCacheKey(username));
            if (item == null || ((string[])item).Length == 0)
            {
                item = System.Web.Security.Roles.GetRolesForUser(username);
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new TimeSpan(0, 10, 0);
                //policy.ChangeMonitors.Add(new SqlChangeMonitor(new System.Data.SqlClient.SqlDependency()))
                cache.Add(GetUserRoleCacheKey(username), item, policy);
            }
            return (string[])item;
        }
        public static string[] GetRolesForUser()
        {
            return GetRolesForUser(HttpContext.Current.User.Identity.Name);
        }

        private static string GetUserRoleCacheKey(string username)
        {
            return "UserRoles-" + username;
        }
        public static bool IsUserInRole(string role)
        {
            return IsUserInRole(HttpContext.Current.User.Identity.Name, role);
        }
        public static bool IsUserInRole(string userName, string role)
        {
            role = role.ToLower();
            return Array.Find(GetRolesForUser(userName), a => a.ToLower() == role) != null;
        }
        public static string GetRoleTitle(string role)
        {
            role = role.ToLower();
            return GetAllRoles().First(a => a.Key.ToLower() == role).Value;
        }

        private static Dictionary<string, string> GetAllRoles()
        {
            var roles = cache["AllRoles"] as Dictionary<string, string>;
            if (roles == null)
            {
                var rs = UniRoles.GetAllRoles();
                roles = new Dictionary<string, string>();
                foreach (var rr in rs)
                    roles.Add(rr.RoleName, rr.RoleTitle);
                cache["AllRoles"] = roles;
            }
            return roles;
        }

        public static bool RoleExists(string name)
        {
            name = name.ToLower();
            return GetAllRoles().Select(a => a.Key.ToLower()).Contains(name);
        }

        public static void AddUserToRole(string userName, string rolename)
        {
            cache.Remove(GetUserRoleCacheKey(userName));
            System.Web.Security.Roles.AddUserToRole(userName, rolename);
        }

        public static void RemoveUserFromRole(string userName, string r)
        {
            cache.Remove(GetUserRoleCacheKey(userName));
            System.Web.Security.Roles.RemoveUserFromRole(userName, r);
        }

        public static void AddUserToRoles(string userName, string[] roles)
        {
            cache.Remove(GetUserRoleCacheKey(userName));
            if (roles.Length > 0)
                System.Web.Security.Roles.AddUserToRoles(userName, roles);
        }

    }
}
