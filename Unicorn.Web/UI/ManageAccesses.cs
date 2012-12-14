using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unicorn.Web.Security.Authorization;
using System.Web.Security;
using Unicorn.Web.Security;

namespace Unicorn.Web.UI
{
    [ToolboxData("<{0}:JcoManageAccesses runat=server></{0}:JcoManageAccesses>")]
    public class ManageAccesses : CompositeControl
    {
        TreeView uxActionsTree;
        ListBox uxUsers, uxRoles;
        Button uxSave;

        Dictionary<string, List<string>> usersActions;
        Dictionary<string, List<string>> rolesActions;

        public TreeView ActionsTree
        {
            get
            {
                EnsureChildControls();
                return uxActionsTree;
            }
        }
        //public string ActionPrefix
        //{
        //    get { return (string)(ViewState["prefix"] ?? ""); }
        //    set { ViewState["prefix"] = value; }
        //}

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            uxActionsTree = new TreeView();
            uxActionsTree.ID = "uxActionsTree";
            uxActionsTree.ShowCheckBoxes = TreeNodeTypes.All;
            uxActionsTree.PathSeparator = '.';
            uxActionsTree.Style["text-align"] = "right";
            Controls.Add(uxActionsTree);

            uxUsers = new ListBox();
            uxUsers.Font.Name = "Tahoma";
            uxUsers.ID = "uxUsers";
            uxUsers.Height = 200;
            uxUsers.Width = 200;
            uxUsers.AutoPostBack = true;
            uxUsers.SelectedIndexChanged += new EventHandler(uxUsers_SelectedIndexChanged);
            Controls.Add(uxUsers);

            uxRoles = new ListBox();
            uxRoles.ID = "uxRoles";
            uxRoles.Font.Name = "Tahoma";
            uxRoles.Height = 200;
            uxRoles.Width = 200;
            uxRoles.AutoPostBack = true;
            uxRoles.SelectedIndexChanged += new EventHandler(uxRoles_SelectedIndexChanged);
            Controls.Add(uxRoles);

            uxSave = new Button();
            uxSave.ID = "uxSave";
            uxSave.Text = "ذخيره";
            uxSave.Width = 120;
            uxSave.Height = 30;
            uxSave.Click += new EventHandler(uxSave_Click);
            Controls.Add(uxSave);

            ChildControlsCreated = true;
        }
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }


        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "10");
            writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Direction, "rtl");
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.WriteEncodedText("کاربران:");
            writer.Write("<br/>");
            uxUsers.RenderControl(writer);
            writer.Write("<br/><br/>");
            writer.WriteEncodedText("نقشهاي کاربري:");
            writer.Write("<br/>");
            uxRoles.RenderControl(writer);

            writer.RenderEndTag();//td
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:477px; height:437px; overflow:scroll; border-style:outset; border-width:2px;");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            uxActionsTree.RenderControl(writer);
            writer.RenderEndTag();//td
            writer.RenderEndTag();//tr

            writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            uxSave.RenderControl(writer);
            writer.RenderEndTag();//td
            writer.RenderEndTag();//tr

        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            if (!Page.IsPostBack)
            {
                //if (UseSiteMapFileForActions)
                //{

                //}
                //else 
                //if (AuthorizationManager.Actions.Count == 0)
                //    AuthorizationManager.AddActionsFromSiteMap();
                //MakeTree(uxActionsTree, AuthorizationManager.Actions);
                uxUsers.DataSource = Membership.GetAllUsers();
                uxUsers.DataValueField = "UserName";
                uxUsers.DataBind();
                uxRoles.DataSource = UniRoles.GetAllRoles();
                uxRoles.DataValueField = "RoleName";
                uxRoles.DataTextField = "RoleTitle";
                uxRoles.DataBind();

                usersActions = new Dictionary<string, List<string>>();
                rolesActions = new Dictionary<string, List<string>>();
                ViewState["_rolesActions_"] = rolesActions;
                ViewState["_usersActions_"] = usersActions;
            }
            else
            {
                rolesActions = (Dictionary<string, List<string>>)ViewState["_rolesActions_"];
                usersActions = (Dictionary<string, List<string>>)ViewState["_usersActions_"];
            }

            base.OnLoad(e);
        }
        //protected override void OnPreRender(EventArgs e)
        //{
        //    base.OnPreRender(e);
        //    if (!Page.IsPostBack)
        //    {
        //        if( !string.IsNullOrEmpty(ActionPrefix))
        //        foreach (TreeNode node in uxActionsTree.Nodes)
        //            node.Value = ActionPrefix + "." + node.Value;
        //    }
        //}

        string[] GetRoleActions(string role)
        {
            if (rolesActions.ContainsKey(role))
                return rolesActions[role].ToArray();
            return AuthorizationManager.GetRoleActions(role);
        }
        string[] GetUserActions(string user)
        {
            if (usersActions.ContainsKey(user))
                return usersActions[user].ToArray();
            return AuthorizationManager.GetUserActions(user);
        }

        private AuthorizedAction[] GetActions(List<string> list)
        {
            List<AuthorizedAction> actions = new List<AuthorizedAction>();
            foreach (string s in list)
            {
                actions.Add(new AuthorizedAction(s));
            }
            return actions.ToArray();
        }
        protected void uxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePrev();
            uxRoles.SelectedIndex = -1;
            ClearActionChecks();
            string[] actions = GetUserActions(uxUsers.SelectedValue);
            foreach (string ac in actions)
            {
                CheckNode(FindNode(uxActionsTree.Nodes, ac.Name), ac);
            }
            ViewState["prevUser"] = uxUsers.SelectedValue;
            ViewState.Remove("prevRole");
        }

        protected void uxRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePrev();
            uxUsers.SelectedIndex = -1;
            ClearActionChecks();
            string[] actions = GetRoleActions(uxRoles.SelectedValue);
            foreach (string ac in actions)
            {
                CheckNode(FindNode(uxActionsTree.Nodes, ac.Name), ac);
            }
            ViewState["prevRole"] = uxRoles.SelectedValue;
            ViewState.Remove("prevUser");
        }

        private void ClearActionChecks()
        {
            while (uxActionsTree.CheckedNodes.Count > 0)
            {
                uxActionsTree.CheckedNodes[0].Checked = false;
            }
        }

        private TreeNode FindNode(TreeNodeCollection nodes, string value)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Value == value)
                    return node;
            }
            return null;
        }
        private void CheckNode(TreeNode node, string ac)
        {
            if (node == null)
                return;
            if (ac.SubActions.Count == 0)
                node.Checked = true;
            else
                foreach (AuthorizedAction item in ac.SubActions)
                {
                    TreeNode ch = FindNode(node.ChildNodes, item.Name);
                    if (ch != null)
                        CheckNode(ch, item);
                }
        }

        private void SavePrev()
        {
            if (ViewState["prevUser"] != null)
                Save(usersActions, (string)ViewState["prevUser"]);
            else if (ViewState["prevRole"] != null)
                Save(rolesActions, (string)ViewState["prevRole"]);
        }
        private void Save(Dictionary<string, List<string>> actionsList, string prev)
        {
            List<string> actions = new List<string>();
            foreach (TreeNode checkedNode in uxActionsTree.CheckedNodes)
            {
                actions.Add(checkedNode.ValuePath);
            }
            actionsList[prev] = actions;
        }

        private void SaveCurrent()
        {
            if (!string.IsNullOrEmpty(uxUsers.SelectedValue))
                Save(usersActions, uxUsers.SelectedValue);
            else if (!string.IsNullOrEmpty(uxRoles.SelectedValue))
                Save(rolesActions, uxRoles.SelectedValue);
        }
        protected void uxSave_Click(object sender, EventArgs e)
        {
            SaveCurrent();
            foreach (string user in usersActions.Keys)
            {
                foreach (TreeNode n in ActionsTree.Nodes)
                    AuthorizationManager.ClearUserActions(user, n.Value);
                foreach (string action in usersActions[user])
                    AuthorizationManager.AddActionForUser(user, action);
            }
            foreach (string role in rolesActions.Keys)
            {
                foreach (TreeNode n in ActionsTree.Nodes)
                    AuthorizationManager.ClearRoleActions(role, n.Value);
                foreach (string action in rolesActions[role])
                    AuthorizationManager.AddActionForRole(role, action);
            }
            Unicorn.Web.WebUtility.ShowMessageBox("کاربر گرامی اطلاعات شما ذخیره شد .", Page);
        }

    }
}
