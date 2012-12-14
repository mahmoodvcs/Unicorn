using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Profile;

namespace Unicorn.Web.Security.Authorization
{
    public static class AuthorizationChecker
    {
        public static bool HasEntityAccess(string entityName, bool checkAnyAccessToSubActions = false)
        {
            var user = HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated)
                return false;
            return HasEntityAccess(user.Name, entityName, checkAnyAccessToSubActions);
        }
        public static bool HasEntityAccess(string userName, string entityName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName, checkAnyAccessToSubActions);
        }
        public static bool HasEntityViewAccess(string userName, string entityName, string fieldName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName + ".View." + fieldName, checkAnyAccessToSubActions);
        }
        public static bool HasEntityViewAccess(string userName, string entityName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName + ".View", checkAnyAccessToSubActions);
        }
        public static bool HasEntityEditAccess(string userName, string entityName, string fieldName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName + ".Edit." + fieldName, checkAnyAccessToSubActions);
        }
        public static bool HasEntityEditAccess(string userName, string entityName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName + ".Edit", checkAnyAccessToSubActions);
        }
        public static bool HasEntityDeleteAccess(string userName, string entityName, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(userName, "Entity", entityName + ".Delete", checkAnyAccessToSubActions);
        }
        private static bool HasAccess(string username, string actionCategory, string subAction, bool checkAnyAccessToSubActions = false)
        {
            //if (checkAnyAccessToCategory && (int)Unicorn.Data.SqlHelper.ExecuteScaler("select count(*) from aspnet_UserRoleActions where ActionName like '" + actionCategory + "%'") == 0)
            //    return true;
            return HasAccess(username, actionCategory + "." + subAction, checkAnyAccessToSubActions);
        }
        public static bool HasAccess(string action, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(HttpContext.Current.User.Identity.Name, action, checkAnyAccessToSubActions);
        }
        public static bool HasAccess(string userName, string action, bool checkAnyAccessToSubActions = false)
        {
            if (string.IsNullOrEmpty(userName))
                return false;
            string[] allActions = AuthorizationManager.GetAllActionsForUser(userName);
            if (Array.IndexOf(allActions, action) >= 0)
                return true;
            if (allActions.Any(a => action.StartsWith(a + ".")))
                return true;
            return checkAnyAccessToSubActions && allActions.Any(a => a.StartsWith(action + "."));

        }

    }
}
