using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web.Security.Authorization;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:JcoManageMenuAccesses runat=server></{0}:JcoManageMenuAccesses>")]
    public class ManageMenuAccesses : CompositeControl
    {
        public ManageMenuAccesses()
        {
        }
        ManageAccesses uxAccesses;

        protected override void CreateChildControls()
        {
            uxAccesses = new ManageAccesses();
            //uxAccesses.ActionPrefix = "Menu";
            Controls.Add(uxAccesses);

            base.CreateChildControls();
            ChildControlsCreated = true;
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (AuthorizationManager.Actions.SubActions.Count == 0)
                    AuthorizationManager.AddActionsFromSiteMap(AuthorizationManager.Actions);
                MakeTree(uxAccesses.ActionsTree, AuthorizationManager.Actions);
            }
            base.OnLoad(e);
        }

        private void MakeTree(TreeView trv, AuthorizedAction actions)
        {
            trv.Nodes.Clear();
            //trv.Nodes.Add(new TreeNode("صفحات", "Menu"));
            AddNodes(trv.Nodes, actions);
        }
        private void AddNodes(TreeNodeCollection nodes, AuthorizedAction actions)
        {
            foreach (AuthorizedAction ac in actions.SubActions)
            {
                TreeNode tn = new TreeNode();
                tn.Text = !string.IsNullOrEmpty(ac.Title) ? ac.Title : ac.Name;
                tn.Value = ac.Name;
                nodes.Add(tn);
                if (tn.Depth == 0)
                    tn.Value = tn.Value;

                AddNodes(tn.ChildNodes, ac);
            }
        }
    }
}
