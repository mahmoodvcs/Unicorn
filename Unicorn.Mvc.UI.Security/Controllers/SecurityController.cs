using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unicorn.Mvc.Controllers;
using Unicorn.Mvc.UI;
using Unicorn.Mvc.UI.Models;
using Unicorn.Web.Security.Authorization;

namespace Unicorn.Mvc.UI.Controllers
{
    public class Unicorn_SecurityController : UniControllerBase
    {
        public Unicorn_SecurityController()
        {
            JsonResultSettings.UseCamelCaseNames = true;
        }

        public ActionResult Index()
        {
            return Content("das sec");
        }

        public JsonResult GetActions(string parentAction, string userOrRoleId)
        {
            var rootAction = AuthorizationManager.FindAction(parentAction);
            List<AccessNodeModel> list = new List<AccessNodeModel>();
            var authorized = string.IsNullOrEmpty(userOrRoleId) ? new string[0] : AuthorizationManager.GetActions(userOrRoleId,
                rootAction == AuthorizationManager.Actions ? null : rootAction.FullName);
            foreach (var ch in rootAction.SubActions)
            {
                var ac = new AccessNodeModel
                {
                    Action = ch.Name,
                    Title = ch.Title,
                };
                var fullName = ch.FullName;
                if (authorized.Any(a => a == fullName))
                    ac.Access = NodeAccessType.HasAccess;
                else if (authorized.Any(a => a.StartsWith(fullName + ".")))
                    ac.Access = NodeAccessType.SubNodes;
                else
                    ac.Access = NodeAccessType.NoAccess;
                ac.HasChildren = ch.SubActions.Any();
                list.Add(ac);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAccesses(string userOrRoleId, string parentAction, List<AccessNodeSaveModel> accesses)
        {
            SecurityManager.SaveUserOrRoleAcesses(userOrRoleId, parentAction, accesses);
            return Json(new { ok = true });
        }

    }
}