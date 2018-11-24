using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Unicorn.Web.Security.Authorization;
using Unicorn.Mvc.UI.Models;

namespace Unicorn.Mvc.UI
{
    public static class SecurityManager
    {
        public static void SaveUserOrRoleAcesses(string userOrRoleId, string parentAction, string receivedValue)
        {
            SaveUserOrRoleAcesses(userOrRoleId, parentAction, Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessNodeSaveModel>>(receivedValue));
        }
        public static void SaveUserOrRoleAcesses(string userOrRoleId, string parentAction, List<AccessNodeSaveModel> accesses)
        {
            AddActions(userOrRoleId, parentAction, accesses);
            AuthorizationManager.ClearAuthorizationCache();

        }
        static void AddActions(string userOrRoleId, string parentAction, List<AccessNodeSaveModel> childActions)
        {
            if (childActions == null)
                return;
            foreach (var ac in childActions)
            {
                var action = ac.Action;
                if (!string.IsNullOrEmpty(parentAction))
                    action = parentAction + "." + action;
                if (ac.Access == NodeAccessType.HasAccess)
                {
                    AuthorizationManager.RevokeAccess(userOrRoleId, action);
                    AuthorizationManager.AddAction(userOrRoleId, action);
                }
                else if (ac.Access == NodeAccessType.NoAccess)
                    AuthorizationManager.RevokeAccess(userOrRoleId, action);
                else if (ac.SubActions != null)
                {
                    AuthorizationManager.RevokeAccess(userOrRoleId, action, false);
                    AddActions(userOrRoleId, action, ac.SubActions);
                }
            }
        }
    }
}
