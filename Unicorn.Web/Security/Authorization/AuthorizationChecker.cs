﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Profile;
using Common.Logging;

namespace Unicorn.Web.Security.Authorization
{
    public static class AuthorizationChecker
    {
        static Common.Logging.ILog log;
        static AuthorizationChecker()
        {
        }
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
            var ret = false;
            string s = "";
            if (!string.IsNullOrEmpty(userName))
            {
                string[] allActions = AuthorizationManager.GetAllActionsForUser(userName);
                if (Array.IndexOf(allActions, action) >= 0)
                    ret = true;
                else if (allActions.Any(a => action.StartsWith(a + ".")))
                    ret = true;
                else
                {
                    ret = checkAnyAccessToSubActions && allActions.Any(a => a.StartsWith(action + "."));
                    s = " - check through sub action";
                }
            }
            if (log == null)
                log = LogManager.GetCurrentClassLogger();
            log.Trace(m => m("User '" + userName + "'. action: '" + action + "'. Access: " + ret.ToString() + s));
            return ret;
        }
        public static bool HasAccess<EnumType>(EnumType action, bool checkAnyAccessToSubActions = false)
        {
            return HasAccessForEnum(typeof(EnumType).Name, action.ToString(), checkAnyAccessToSubActions);
        }
        public static bool HasAccess<EnumType>(string parentAction, EnumType action, bool checkAnyAccessToSubActions = false)
        {
            return HasAccessForEnum(parentAction + "." + typeof(EnumType).Name, action.ToString(), checkAnyAccessToSubActions);
        }
        private static bool HasAccessForEnum(string parentAction, string action, bool checkAnyAccessToSubActions = false)
        {
            return HasAccess(parentAction + "." + action.Replace('_', '.'), checkAnyAccessToSubActions);
        }
        public static bool HasAccess<EnumType>(string userName, string parentAction, EnumType action, bool checkAnyAccessToSubActions = false)
        //where EnumType : enum
        {
            string ac = typeof(EnumType).Name + "." + action.ToString().Replace('_', '.');
            if (!string.IsNullOrEmpty(parentAction))
                ac = parentAction + "." + ac;
            if (!string.IsNullOrEmpty(userName))
                return HasAccess(userName, ac, checkAnyAccessToSubActions);
            else
                return HasAccess(ac, checkAnyAccessToSubActions);
        }
    }
}
